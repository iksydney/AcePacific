using Newtonsoft.Json;

namespace AcePacific.Data.ViewModel
{
    public class WalletViewModel
    {
        public decimal WalletBalance { get; set; }
        public string UserId { get; set; }
        public string TransactionReference { get; set; }
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
        public string? RecipientWalletAccountNumber { get; set; }
        public string? SenderWalletAccountNumber { get; set; }
        public string TransactionNarration { get; set; }
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
        public string Pin { get; set; }
        public string UserId { get; set; }
    }
}
