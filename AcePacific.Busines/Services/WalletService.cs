using AcePacific.Common.Constants;
using AcePacific.Common.Contract;
using AcePacific.Common.Helpers;
using AcePacific.Data.Entities;
using AcePacific.Data.Migrations;
using AcePacific.Data.Repositories;
using AcePacific.Data.ViewModel;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Data.Entity;

namespace AcePacific.Busines.Services
{
    public interface IWalletService
    {
        Task<Response<CountModel<WalletItem>>> GetCount(int page, int pagesize, WalletFilter filter = null);
        Task<IEnumerable<WalletItem>> Query(int page, int pagesize, WalletFilter filter);
        Task<Response<GetWalletResponse>> GetWallet(string accountNumber);
        Task<Response<GetWalletResponse>> GetWalletAdmin(string accountNumber);
        Task<Response<string>> IntraTransfer(IntraTransferDto model);
        Task<Response<string>> ResetPin(UpdatePinModel model);
        Task<Response<string>> InterTransfer(InterTransferDto model);
        Task<Response<string>> ValidatePin(ValidatePinModel model);
        Task<Response<string>> CreatePin(ValidatePinModel model);
        Task<Response<TransactionHistoryView>> GettransactionByReference(string reference);
        Task<Response<IEnumerable<TransactionHistoryView>>> ViewUserTransactionHistory(string userId);
        Task<Response<AdminPendingTransactions>> GetAdminPendingTransactionsById(int id);
        Task<Response<string>> ApproveAdminPendingTransactionsById(int id);
        Task<Response<List<AdminPendingTransactions>>> ViewAdminPendingTransactionsHistory();
    }
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ITransactionLogRepository _transactionLogRepository;
        private readonly IAdminPendingTransactionsRepository _adminPendingTransactionsRepository;
        private readonly ILogger<WalletService> _logger;
        public WalletService(IWalletRepository walletRepository, IUserRepository userRepository, IMapper mapper, ITransactionLogRepository transactionLogRepository,
            ILogger<WalletService> logger, IAdminPendingTransactionsRepository adminPendingTransactionsRepository)
        {
            _walletRepository = walletRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _transactionLogRepository = transactionLogRepository;
            _logger = logger;
            _adminPendingTransactionsRepository = adminPendingTransactionsRepository;

        }
        public async Task<Response<string>> IntraTransfer(IntraTransferDto model)
        {
            var response = Response<string>.Failed(string.Empty);
            try
            {
                var accountDetails = _walletRepository.GetWalletByAccountNumber(model.RecipientWalletAccountNumber);
                var recipientUserName = _userRepository.Table.AsNoTracking().FirstOrDefault(x => x.AccountNumber == model.RecipientWalletAccountNumber);
                if(recipientUserName == null)
                {
                    return Response<string>.Failed("Recipirnt details is needed");
                }
                var senderUserName = _userRepository.Table.AsNoTracking().FirstOrDefault(x => x.AccountNumber == model.SenderWalletAccountNumber);
                var senderDetails = _walletRepository.GetWalletByAccountNumber(model.SenderWalletAccountNumber);

                if (accountDetails == null)
                    return Response<string>.Failed(ErrorMessages.AccountNumberDoesntExist);
                if (senderDetails.Pin == null)
                    return Response<string>.Failed(ErrorMessages.PinNotSetup);
                if (senderDetails.WalletAccountNumber == model.RecipientWalletAccountNumber)
                    return Response<string>.Failed("Hmmm!!! You cannot make transfers to same account number 😊");
                if (senderDetails.WalletBalance <= model.Amount)
                    return Response<string>.Failed(ErrorMessages.BalanceIsLow);
                var balanceValidation = ValidateAmount(senderDetails.WalletBalance, model.Amount);
                if (string.IsNullOrEmpty(balanceValidation)) return Response<string>.Failed(ErrorMessages.FailedToValidateBalance);
                if (balanceValidation != ResponseMessage.ValidationSuccessfulTransferCanProceed)
                    return Response<string>.Failed("Please contact Administrator");

                var comparedPin = CompareHashedStrings(model.TransactionPin, senderDetails.Pin);
                if (!comparedPin.Result.Equals("Pin Verified"))
                    return Response<string>.Failed(ErrorMessages.IncorrectPin);
                var amountChanged = senderDetails.WalletBalance;


                

                var transactionReference = Helper.GenerateTransactionReference();

                var transactionLog = new SaveTransactionLog
                {
                    DateCreated = DateTime.UtcNow,
                    Reference = transactionReference,
                    SenderUserId = senderDetails.UserId,
                    RecipientUserId = recipientUserName.Id,
                    RecipientAccountName = recipientUserName?.AccountName,
                    SenderAccountName = senderUserName?.AccountName,
                    TransactionNarration = model.TransactionNarration,
                    TransactionType = TransactionType.InternalTransaction,
                    TransactionAmount = model.Amount.ToString(),
                    CreatedBy = senderUserName?.AccountName,
                    AdminStatus = false
                };
                var mappedTransactionLog = _mapper.Map<TransactionLog>(transactionLog);
                await _transactionLogRepository.InsertAsync(mappedTransactionLog);


                /* senderDetails.WalletBalance -= model.Amount;
                 await _walletRepository.UpdateAsync(senderDetails);

                 accountDetails.WalletBalance += model.Amount; 
                 await _walletRepository.UpdateAsync(accountDetails);

                 response = Response<string>.Success("Transfer completed Successfully");*/

                var pendingApproval = new AdminPendingTransactions()
                {
                    DateCreated = DateTime.Now,
                    FromAccountName = senderUserName?.AccountName,
                    ToAccountName = recipientUserName.AccountName,
                    TransactionAmount = model.Amount,
                    TransactionNarration = model.TransactionNarration,
                    ApproveTransaction = false,
                    TransactionIdPending = mappedTransactionLog.Id,
                    FromAccountNumber = senderDetails.WalletAccountNumber,
                    ToAccountNumber = recipientUserName.AccountNumber
                };

                await _adminPendingTransactionsRepository.InsertAsync(pendingApproval);

                response = Response<string>.Success("Transfer Pending Approval");
            }
            catch (Exception ex)
            {
                response = Response<string>.Failed(ex.Message);
            }
            return await Task.FromResult(response);
        }
        public async Task<Response<string>> ValidatePin(ValidatePinModel model)
        {
            var response = Response<string>.Failed(string.Empty);
            try
            {
                if (string.IsNullOrEmpty(model.UserId))
                    return Response<string>.Failed(ErrorMessages.UserNotFound);
                var pinDetails = _walletRepository.Table.AsNoTracking().FirstOrDefault(c => c.UserId == model.UserId);
                if (pinDetails.Pin == null)
                    return Response<string>.Failed("You have not created a transaction Pin");
                var comparedPin = CompareHashedStrings(model.Pin, pinDetails.Pin);
                if (!comparedPin.Result.Equals("Pin Verified"))
                    return Response<string>.Failed(ErrorMessages.IncorrectPin);

                response = Response<string>.Success("Pin Verified");
            }
            catch (Exception ex)
            {
                response = Response<string>.Failed(ex.Message);
            }
            return await Task.FromResult(response);
        }
        public async Task<Response<string>> InterTransfer(InterTransferDto model)
        {
            var response = Response<string>.Failed(string.Empty);
            try
            {
                //var accountDetails = _walletRepository.GetWalletByAccountNumber("0684109312");
                var senderUserName = _userRepository.Table.AsNoTracking().FirstOrDefault(x => x.AccountNumber == model.SenderAccountNumber);
                if (senderUserName == null)
                    return Response<string>.Failed("Information of the Sender needed");
                var senderDetails = _walletRepository.GetWalletByAccountNumber(model.SenderAccountNumber);
                if (senderDetails == null)
                    return Response<string>.Failed("details not correctly inputted");

                if (senderDetails.Pin == null)
                    return Response<string>.Failed(ErrorMessages.PinNotSetup);

                if (senderDetails.WalletAccountNumber == model.AccountNumber)
                    return Response<string>.Failed("Hmmm!!! You cannot make transfers to same account number 😊");

                if (senderDetails.WalletBalance <= model.TransactionAmount)
                    return Response<string>.Failed(ErrorMessages.BalanceIsLow);

                var balanceValidation = ValidateAmount(senderDetails.WalletBalance, model.TransactionAmount);
                if (string.IsNullOrEmpty(balanceValidation)) return Response<string>.Failed(ErrorMessages.FailedToValidateBalance);

                if (balanceValidation != ResponseMessage.ValidationSuccessfulTransferCanProceed)
                    return Response<string>.Failed("Please contact Administrator");

                var comparedPin = CompareHashedStrings(model.TransactionPin, senderDetails.Pin);
                if (!comparedPin.Result.Equals("Pin Verified"))

                    return Response<string>.Failed(ErrorMessages.IncorrectPin);

                var amountChanged = senderDetails.WalletBalance;


                //senderDetails.WalletBalance -= model.TransactionAmount;

                var transactionReference = Helper.GenerateTransactionReference();

                var transactionLog = new SaveTransactionLog
                {
                    DateCreated = DateTime.UtcNow,
                    Reference = transactionReference,
                    SenderUserId = senderDetails.UserId,
                    SenderAccountName = senderUserName?.AccountName,
                    TransactionAmount = model.TransactionAmount.ToString(),
                    CreatedBy = senderUserName?.AccountName,
                    RecipientAccountName = model.AccountName,
                    TransactionNarration = model.TransactionNarration,
                    TransactionType = TransactionType.ExternalTransaction,
                    PostalCode = model.PostalCode,
                    SenderAddress = model.SenderAddress,
                    RoutingNumber = model.RoutingNumber,
                    SwiftCode = model.SwiftCode,
                    BankName = model.BankName,
                    AdminStatus = false
                };
                var mappedTransactionLog = _mapper.Map<TransactionLog>(transactionLog);


                await _transactionLogRepository.InsertAsync(mappedTransactionLog);


                //await _walletRepository.UpdateAsync(senderDetails);

                //await _walletRepository.UpdateAsync(accountDetails);

                var pendingApproval = new AdminPendingTransactions()
                {
                    DateCreated = DateTime.Now,
                    FromAccountName = senderUserName?.AccountName,
                    ToAccountName = model.AccountName,
                    SwiftCode = model.SwiftCode,
                    RoutingNumber = model.RoutingNumber,
                    TransactionAmount = model.TransactionAmount,
                    TransactionNarration = model.TransactionNarration,
                    SenderAddress = model.SenderAddress,
                    PostalCode = model.PostalCode,
                    ApproveTransaction = false,
                    TransactionIdPending = mappedTransactionLog.Id,
                    FromAccountNumber = model.SenderAccountNumber
                };

                await _adminPendingTransactionsRepository.InsertAsync(pendingApproval);

                response = Response<string>.Success("Transfer Pending Approval");
            }
            catch (Exception ex)
            {
                response = Response<string>.Failed(ex.Message);
            }
            return await Task.FromResult(response);
        }

        public async Task<Response<AdminPendingTransactions>> GetAdminPendingTransactionsById(int id)
        {
            var response = Response<AdminPendingTransactions>.Failed(string.Empty);
            try
            {
                var entity = _adminPendingTransactionsRepository.Table.FirstOrDefault(x => x.Id == id);
                if(entity  == null)
                    return Response<AdminPendingTransactions>.Failed("transaction Not found");

                response = Response<AdminPendingTransactions>.Success(entity);
            }
            catch (Exception ex)
            {
                response = Response<AdminPendingTransactions>.Failed(ex.Message);
            }
            return await Task.FromResult(response);
        }
        public async Task<Response<string>> ApproveAdminPendingTransactionsById(int id)
        {
            var response = Response<string>.Failed(string.Empty);
            try
            {
                var receiversWallet = new Wallet();
                var entity = _adminPendingTransactionsRepository.Table.FirstOrDefault(x => x.Id == id);
                if(entity  == null)
                    return Response<string>.Failed("transaction Not found");

                if (entity.ApproveTransaction == true)
                    return Response<string>.Failed("Transaction Already approved");


                var senderDetails = _userRepository.Table.FirstOrDefault(x => x.AccountNumber == entity.FromAccountNumber);
                if (senderDetails == null)
                    return Response<string>.Failed("User not found");

                var sendersWallet = _walletRepository.Table.FirstOrDefault(x => x.WalletAccountNumber == senderDetails.AccountNumber);
                if (senderDetails == null)
                    return Response<string>.Failed("User not found");
                if(entity.ToAccountNumber != null)
                {
                    receiversWallet = _walletRepository.Table.FirstOrDefault(c => c.WalletAccountNumber.Equals(entity.ToAccountNumber));

                    sendersWallet.WalletBalance -= entity.TransactionAmount;
                    await _walletRepository.UpdateAsync(sendersWallet);

                    receiversWallet.WalletBalance += entity.TransactionAmount;
                    await _walletRepository.UpdateAsync(sendersWallet);
                }
                else
                {
                    sendersWallet.WalletBalance -= entity.TransactionAmount;
                    await _walletRepository.UpdateAsync(sendersWallet);
                }

                var transactionLogEntity = _transactionLogRepository.Table.FirstOrDefault(x => x.Id == entity.TransactionIdPending);
                if (transactionLogEntity == null)
                    return Response<string>.Failed("transaction log Not found");
                transactionLogEntity.AdminStatus = true;
                await _transactionLogRepository.UpdateAsync(transactionLogEntity);

                entity.ApproveTransaction = !entity.ApproveTransaction;

                await _adminPendingTransactionsRepository.UpdateAsync(entity);
                response = Response<string>.Success("Transaction Updated Successfully");
            }
            catch (Exception ex)
            {
                response = Response<string>.Failed(ex.Message);
            }
            return await Task.FromResult(response);
        }
        public async Task<Response<List<AdminPendingTransactions>>> ViewAdminPendingTransactionsHistory()
        {
            var response = Response<List<AdminPendingTransactions>>.Failed(string.Empty);
            try
            {
                var today = DateTime.UtcNow;
                var oneMonthAgo = today.AddMonths(-11);
                var entity = _adminPendingTransactionsRepository.Table.OrderByDescending(x => x.DateCreated).ToList();


                response = Response<List<AdminPendingTransactions>>.Success(entity);

            }
            catch (Exception ex)
            {
                response = Response<List<AdminPendingTransactions>>.Failed(ex.Message);
                _logger.LogError(ex.Message);
            }
            return await Task.FromResult(response).ConfigureAwait(false);
        }
        public async Task<Response<IEnumerable<TransactionHistoryView>>> ViewUserTransactionHistory(string userId)
        {
            var response = Response<IEnumerable<TransactionHistoryView>>.Failed(string.Empty);
            try
            {
                var today = DateTime.UtcNow;
                var FilteredDate = today.AddDays(-365);
                var entity = _transactionLogRepository.Table.Where(x => x.RecipientUserId == userId || x.SenderUserId == userId).OrderByDescending(x => x.DateCreated).ToList();
                
                var mappedEntity = _mapper.Map<IEnumerable<TransactionHistoryView>>(entity);

                response = Response<IEnumerable<TransactionHistoryView>>.Success(mappedEntity);

            }catch(Exception ex)
            {
                response = Response<IEnumerable<TransactionHistoryView>>.Failed(ex.Message);
                _logger.LogError(ex.Message);
            }
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        public async Task<Response<TransactionHistoryView>> GettransactionByReference(string reference)
        {
            var response = Response<TransactionHistoryView>.Failed(string.Empty);
            try
            {
                var entity = _transactionLogRepository.Table.AsNoTracking().FirstOrDefault(x => x.Reference == reference);

                var mappedEntity = _mapper.Map<TransactionHistoryView>(entity);

                response = Response<TransactionHistoryView>.Success(mappedEntity);
            }catch(Exception ex)
            {
                response = Response<TransactionHistoryView>.Failed(ex.Message);
                _logger.LogError(ex.Message);
            }
            return await Task.FromResult(response);
        }
        private string ValidateAmount(decimal? balance, decimal amountToSend)
        {
            if (amountToSend <= 0)
                return ErrorMessages.InvalidTransferAmount;
            if (balance < amountToSend)
                return $"Insufficient funds. Balance: {balance:C}. Transfer amount: {amountToSend:C}.);";

            return ResponseMessage.ValidationSuccessfulTransferCanProceed;
        }
        private async Task<Response<string>> ValidateUserBalance(string accountNumber, decimal amount)
        {
            var response = Response<string>.Failed("Failed to validate amount");
            try
            {
                var accountDetails = _userRepository.FindByAccountNumber(accountNumber);
            }
            catch (Exception ex)
            {
                response = Response<string>.Failed(ex.Message);
            }
            return await Task.FromResult(response);
        }
        public async Task<Response<GetWalletResponse>> GetWallet(string accountNumber)
        {
            var response = Response<GetWalletResponse>.Failed(string.Empty);
            try
            {
                var accountDetails = _userRepository.FindByAccountNumber(accountNumber);
                if (accountDetails == null)
                    return Response<GetWalletResponse>.Failed(ErrorMessages.FailedToRetrieveWallet);

                //Guid walletId = new Guid(accountDetails.Id);
                var walletDetails = _walletRepository.GetWalletById(accountDetails.Id);
                if (walletDetails == null)
                    return Response<GetWalletResponse>.Failed(ErrorMessages.FailedToRetrieveWallet);
                response = Response<GetWalletResponse>.Success(new GetWalletResponse
                {
                    FirstName = accountDetails.FirstName,
                    LastName = accountDetails.LastName,
                    AccountNumber = walletDetails.WalletAccountNumber,
                });
            }
            catch (Exception ex)
            {
                response = Response<GetWalletResponse>.Failed(ex.Message);
            }
            return await Task.FromResult(response).ConfigureAwait(false);
        }
        public async Task<Response<GetWalletResponse>> GetWalletAdmin(string accountNumber)
        {
            var response = Response<GetWalletResponse>.Failed(string.Empty);
            try
            {
                var accountDetails = _userRepository.FindByAccountNumber(accountNumber);
                if (accountDetails == null)
                    return Response<GetWalletResponse>.Failed(ErrorMessages.AccountNumberDoesntExist);
                Guid accountId = new Guid(accountDetails.Id);
                var walletDetails = _walletRepository.GetWalletById(accountDetails.Id);
                response = Response<GetWalletResponse>.Success(new GetWalletResponse
                {
                    FirstName = accountDetails.FirstName,
                    LastName = accountDetails.LastName,
                    AccountNumber = walletDetails.WalletAccountNumber,
                    AccountBalance = (decimal)(walletDetails.WalletBalance),
                    Pin = walletDetails.Pin
                });
            }
            catch (Exception ex)
            {
                response = Response<GetWalletResponse>.Failed(ex.Message);
            }
            return await Task.FromResult(response).ConfigureAwait(false);
        }
        public async Task<Response<string>> ResetPin(UpdatePinModel model)
        {
            var walletDetails = _walletRepository.GetWalletByUserId(model.UserId);
            if (walletDetails == null)
                return Response<string>.Failed(ErrorMessages.UserNotFound);
            var hashedOldPin = Helper.ComputeHash(model.OldPin);
            if (hashedOldPin != walletDetails.Pin)
                return Response<string>.Failed(ErrorMessages.PinNotMatch);

            var hashedNewPin = Helper.ComputeHash(model.NewPin);
            walletDetails.Pin = hashedNewPin;

            _walletRepository.Update(walletDetails);
            return Response<string>.Success("Pin updated Successfully");
        }

        public async Task<Response<string>> CreatePin(ValidatePinModel model)
        {
            try
            {
                var walletDetails = _walletRepository.GetWalletByUserId(model.UserId);
                if (walletDetails == null)
                    return Response<string>.Failed(ErrorMessages.UserNotFound);
                if (walletDetails.Pin != null) return Response<string>.Failed("Pin Already Exists");

                walletDetails.Pin = Helper.ComputeHash(model.Pin);
                _walletRepository.Update(walletDetails);

                return Response<string>.Success("Pin Created Successfully");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }
        private async Task<string> CompareHashedStrings(string sourcePin, string hashedPin)
        {
            var comparedHash = Helper.ComputeHash(sourcePin);
            if (!hashedPin.Equals(comparedHash))
            {
                return "Invalid Transaction PIN";
            }
            else
            {
                return await Task.FromResult("Pin Verified");
            }

        }
        public async Task<Response<CountModel<WalletItem>>> GetCount(int page, int pagesize, WalletFilter filter = null)
        {
            ProcessFilter(filter);
            int totalCount;
            var orderBy = OrderExpression.Deserilizer("{}");
            var entities = _walletRepository.GetWalletPaged(page, pagesize, out totalCount, filter, orderBy);

            var response = Response<CountModel<WalletItem>>.Success(new CountModel<WalletItem>()
            {
                Total = totalCount,
                Items = ProcessQuery(entities)
            });
            return await Task.FromResult(response);
        }
        public async Task<IEnumerable<WalletItem>> Query(int page, int pagesize, WalletFilter filter)
        {
            var orderBy = OrderExpression.Deserilizer("{}");
            ProcessFilter(filter);
            var entities = _walletRepository.GetWalletPaged(page, pagesize, filter, orderBy);
            var response = ProcessQuery(entities);
            return await Task.FromResult(response);
        }
        private void ProcessFilter(WalletFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.UserId))
            {
                var wallet = _walletRepository.GetWalletByUserId(filter.UserId);
                if (wallet != null)
                {
                    filter.UserId = wallet.UserId;
                }
            }
        }
        private IEnumerable<WalletItem> ProcessQuery(IEnumerable<Wallet> entities)
        {
            return entities.ToList().Select(c =>
            {
                var item = _mapper.Map<Wallet, WalletItem>(c);
                return item;
            });
        }
    }
}
