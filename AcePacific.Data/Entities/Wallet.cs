using System.ComponentModel.DataAnnotations;

namespace AcePacific.Data.Entities
{
    public class Wallet
    {
        [Key]
        public Guid Id { get; set; }
        public decimal WalletBalance { get; set; }
        public string UserId { get; set; }
        public string TransactionReference { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public string TransactionDate { get; set; }
        public DateTime DateUpdated { get; set; }
        public string WalletAccountNumber { get; set; }
    }
}
