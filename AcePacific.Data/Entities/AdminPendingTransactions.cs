namespace AcePacific.Data.Entities
{
    public class AdminPendingTransactions
    {
        public int Id { get; set; }
        public string? FromAccountName { get; set; }
        public string? FromAccountNumber { get; set; }
        public string? ToAccountNumber { get; set; }
        public string? ToAccountName { get; set; }
        public string? SwiftCode { get; set; }
        public string? RoutingNumber { get; set; }
        public decimal TransactionAmount{ get; set; }
        public string? TransactionNarration{ get; set; }
        public string? SenderAddress{ get; set; }
        public string? PostalCode{ get; set; }
        public bool ApproveTransaction { get; set; } = false;
        public DateTime DateCreated { get; set; }
        public int TransactionIdPending { get; set; }
    }
}
