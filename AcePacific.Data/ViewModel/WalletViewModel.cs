using AcePacific.Data.Entities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AcePacific.Data.ViewModel
{
    public class WalletViewModel
    {
        public decimal WalletBalance { get; set; }
        public string UserId { get; set; }
        public string TransactionReference { get; set; }
        public string WalletAccountNumber { get; set; }
    }   
    public class WalletItem : WalletViewModel
    {
        public DateTime? LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime DateCreated { get; set; }
    }
    public class WalletFilter : WalletItem
    {
        public DateTime? DateCreatedFrom { get; set; }
        public DateTime? DateCreatedTo { get; set; }
        public static WalletFilter Deserialize(string whereCondition)
        {
            var filter = new WalletFilter();
            if (!string.IsNullOrEmpty(whereCondition))
            {
                filter = JsonConvert.DeserializeObject<WalletFilter>(whereCondition);
            }
            return filter;
        }
    }

    public class TransactionHistoryView
    {
        public string? TransactionReference { get; set; }
        public string? Reference { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankImageUrl { get; set; }
        public string? UserId { get; set; }
        public string? CreatedBy { get; set; }
        public string? Sender { get; set; }
        public decimal? Charge { get; set; }
        public string? PhoneNumber { get; set; }
        public string? RecipientAccountName { get; set; }
        public string? SenderAccountName { get; set; }
        public string? TransactionNarration { get; set; }
        public string? SwiftCode { get; set; }
        public string? RoutingNumber { get; set; }
        public string? SenderAddress { get; set; }
        public string? PostalCode { get; set; }
        public TransactionType? TransactionType { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? TransactionAmount { get; set; }
        public bool ApproveTransaction { get; set; }

    }
    public class CreatWalletViewModel
    {
        public string UserId { get; set; }
        public string WalletAccountNumber { get; set; }
        public decimal? WalletBalance { get; set; }
    }
    public class IntraTransferItem
    {
        public string TransactionStatus { get; set; }
    }
    public class IntraTransferDto
    {
        public decimal Amount { get; set;}
        [Required(ErrorMessage ="Please provide a Recipient wallet account number")]
        public string? RecipientWalletAccountNumber { get; set; }
        [Required(ErrorMessage = "Some needed parameters are missing")]
        public string? SenderWalletAccountNumber { get; set; }
        public string TransactionNarration { get; set; }
        public string TransactionPin { get; set; }
    }
    public class InterTransferDto
    {
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string SenderAccountNumber { get; set; }
        public string BankName { get; set; }
        public string SwiftCode { get; set; }
        public string RoutingNumber { get; set; }
        public decimal TransactionAmount { get; set;}
        public string TransactionNarration { get; set; }
        public string? SenderAddress { get; set; }
        public string? PostalCode { get; set; }
        public string TransactionPin { get; set; }    
    }
    public class GetWalletResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountNumber { get; set; }
        public decimal AccountBalance { get; set;}
        public string Pin { get; set;}
    }
    public class UpdatePinModel
    {
        public string OldPin { get; set; }
        public string NewPin { get; set; }
        public string UserId { get; set; }
    }

    public class ValidatePinModel
    {
        public string Pin { get; set; }
        [Required(ErrorMessage ="User Id required")]
        public string UserId { get; set; }
    }
}
