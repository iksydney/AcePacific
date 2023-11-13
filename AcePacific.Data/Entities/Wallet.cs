using System.ComponentModel.DataAnnotations;

namespace AcePacific.Data.Entities
{
    public class Wallet
    {
        [Key]
        public Guid Id { get; set; }
        public decimal? WalletBalance { get; set; }
        public string? UserId { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public string? WalletAccountNumber { get; set; }
        public string? Pin { get; set; }
    }
}
