using AcePacific.Common.Constants;
using AcePacific.Common.Contract;
using AcePacific.Common.Helpers;
using AcePacific.Data.Entities;
using AcePacific.Data.Repositories;
using AcePacific.Data.ViewModel;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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
    }
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ITransactionLogRepository _transactionLogRepository;
        private readonly ILogger<WalletService> _logger;
        public WalletService(IWalletRepository walletRepository, IUserRepository userRepository, IMapper mapper, ITransactionLogRepository transactionLogRepository,
            ILogger<WalletService> logger)
        {
            _walletRepository = walletRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _transactionLogRepository = transactionLogRepository;
            _logger = logger;

        }
        public async Task<Response<string>> IntraTransfer(IntraTransferDto model)
        {
            var response = Response<string>.Failed(string.Empty);
            try
            {
                var accountDetails = _walletRepository.GetWalletByAccountNumber(model.RecipientWalletAccountNumber);
                var senderDetails = _walletRepository.GetWalletByAccountNumber(model.SenderWalletAccountNumber);

                if (accountDetails == null)
                    return Response<string>.Failed(ErrorMessages.AccountNumberDoesntExist);
                if (senderDetails.Pin == null)
                    return Response<string>.Failed(ErrorMessages.PinNotSetup);
                if(senderDetails.WalletAccountNumber == model.RecipientWalletAccountNumber)
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


                senderDetails.WalletBalance -= model.Amount;

                var transactionReference = Helper.GenerateTransactionReference();

                var transactionLog = new TransactionLogItem
                {
                    DateCreated = DateTime.UtcNow,
                    Reference = transactionReference,
                    RecipientAccountName = accountDetails.WalletAccountNumber,
                    SenderAccountName = senderDetails.WalletAccountNumber,
                    TransactionNarration = model.TransactionNarration,
                    TransactionType = TransactionType.InternalTransaction
                };
                var mappedTransactionLog = _mapper.Map<TransactionLog>(transactionLog);
                await _transactionLogRepository.InsertAsync(mappedTransactionLog);

                await _walletRepository.UpdateAsync(senderDetails);
                
                accountDetails.WalletBalance += model.Amount;
                await _walletRepository.UpdateAsync(accountDetails);

                response = Response<string>.Success("Transfer completed Successfully");
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
                if(string.IsNullOrEmpty( model.UserId))
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
                var accountDetails = _walletRepository.GetWalletByAccountNumber("0684109312");
                var senderDetails = _walletRepository.GetWalletByAccountNumber(model.SenderAccountNumber);
                if (senderDetails == null || accountDetails == null)
                    return Response<string>.Failed("details not correctly inputted");

                if (senderDetails.Pin == null)
                    return Response<string>.Failed(ErrorMessages.PinNotSetup);

                if(senderDetails.WalletAccountNumber == model.AccountNumber)
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


                senderDetails.WalletBalance -= model.TransactionAmount;

                var transactionReference = Helper.GenerateTransactionReference();

                var transactionLog = new TransactionLogItem
                {
                    DateCreated = DateTime.UtcNow,
                    Reference = transactionReference,
                    RecipientAccountName = model.AccountName,
                    SenderAccountName = senderDetails.WalletAccountNumber,
                    TransactionNarration = model.TransactionNarration,
                    TransactionType = TransactionType.ExternalTransaction,
                    PostalCode = model.PostalCode,
                    SenderAddress = model.SenderAddress,
                    RoutingNumber = model.RoutingNumber,
                    SwiftCode = model.SwiftCode,
                    BankName = model.BankName
                    
                };
                var mappedTransactionLog = _mapper.Map<TransactionLog>(transactionLog);
                await _transactionLogRepository.InsertAsync(mappedTransactionLog);

                await _walletRepository.UpdateAsync(senderDetails);
                
                accountDetails.WalletBalance += model.TransactionAmount;
                await _walletRepository.UpdateAsync(accountDetails);

                response = Response<string>.Success("Transfer completed Successfully");
            }
            catch (Exception ex)
            {
                response = Response<string>.Failed(ex.Message);
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
