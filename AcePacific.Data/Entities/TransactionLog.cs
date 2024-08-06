using System.ComponentModel;

namespace AcePacific.Data.Entities
{
    public class TransactionLog
    {
        public int Id { get; set; }
        public string? Reference { get; set; }
        public string? RecipientAccountName { get; set; }
        public string? SenderAccountName { get; set; }
        public string? AccountNumber { get; set; }
        public int? BankId { get; set; }
        public string? BankName { get; set; }
        public string? RecipientUserId { get; set; }
        public string? SenderUserId { get; set; }
        public string? CreatedBy { get; set; }
        public string? Sender { get; set; }
        public decimal? Charge { get; set; } = 0m;
        public string? PhoneNumber { get; set; }
        public string? TransactionNarration { get; set; }
        public string? TransactionAmount { get; set; }
        public string? RecipientAddress { get; set; }
        public string? SenderAddress { get; set; }
        public string? PostalCode { get; set; }
        public string? SwiftCode { get; set; }
        public string? RoutingNumber { get; set; }
        public string? IBANNumber { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool AdminStatus { get; set; }
        public TransactionType? TransactionType { get; set; }
    }
    public enum TransactionType
    {
        [Description("External Transaction")]
        ExternalTransaction,
        [Description("Internal Transaction")]
        InternalTransaction,
        [Description("Credit Transaction")]
        CreditTransaction,
        [Description("Loan")]
        Loan,
        [Description("Personal Transaction")]
        PersonalTransaction,
        [Description("Business Transaction")]
        BusinessTransaction,
        [Description("Non Business Transaction")]
        NonBusinessTransaction
    }
}
