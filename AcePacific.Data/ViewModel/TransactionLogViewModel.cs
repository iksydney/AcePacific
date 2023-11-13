using AcePacific.Data.Entities;
using Newtonsoft.Json;

namespace AcePacific.Data.ViewModel
{
    public class TransactionLogModel
    {
        public string? Reference { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? UserId { get; set; }
        public string? CreatedBy { get; set; }
        public string? Sender { get; set; }
        public decimal? Charge { get; set; }
        public string? PhoneNumber { get; set; }
        public string? RecipientAccountName { get; set; }
        public string? SenderAccountName { get; set; }
        public string? TransactionNarration { get; set; }
        public TransactionType? TransactionType { get; set; }
    }
    public class TransactionLogItem : TransactionLogModel
    {
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public DateTime? VerifiedOn { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
    }
    public class TransactionLogFilter : TransactionLogItem
    {
        public DateTime? DateCreatedFrom { get; set; }
        public DateTime? DateCreatedTo { get; set; }
        public static TransactionLogFilter Deserialize(string whereCondition)
        {
            var filter = new TransactionLogFilter();
            if (!string.IsNullOrEmpty(whereCondition))
            {
                filter = JsonConvert.DeserializeObject<TransactionLogFilter>(whereCondition);
            }
            return filter;
        }
    }
}
