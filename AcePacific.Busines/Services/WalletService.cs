using AcePacific.Common.Constants;
using AcePacific.Common.Contract;
using AcePacific.Common.Helpers;
using AcePacific.Data.Entities;
using AcePacific.Data.Repositories;
using AcePacific.Data.ViewModel;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Data.Entity;

namespace AcePacific.Busines.Services
{
    public interface IWalletService
    {
        Task<Response<GetWalletResponse>> GetWallet(string accountNumber);
        Task<Response<GetWalletResponse>> GetWalletAdmin(string accountNumber);
        Task<Response<string>> IntraTransfer(IntraTransferDto model);
        Task<Response<string>> UpdatePin(UpdatePinModel model);
    }
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ITransactionLogRepository _transactionLogRepository;
        public WalletService(IWalletRepository walletRepository, IUserRepository userRepository, IMapper mapper, ITransactionLogRepository transactionLogRepository)
        {
            _walletRepository = walletRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _transactionLogRepository = transactionLogRepository;

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
        public async Task<Response<string>> UpdatePin(UpdatePinModel model)
        {
            var walletDetails = _walletRepository.GetWalletByUserId(model.UserId);
            if (walletDetails == null)
                return Response<string>.Failed(ErrorMessages.UserNotFound);
            walletDetails.Pin = Helper.ComputeHash(model.Pin);
            /*var walletModel = new UpdatePinModel
            {
                Pin = model.Pin
            };
            var mappedWallet = _mapper.Map<Wallet>(walletModel);*/
            _walletRepository.Update(walletDetails);
            return Response<string>.Success("Pin updated Successfully");
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
    }
}
