using System.ComponentModel.DataAnnotations;

namespace AcePacific.Data.Entities
{
    public class OtpStore
    {
        [Key]
        [Required] public int Id { get; set; }
        [Required] public string PhoneNumber { get; set; }
        [Required] public string Otp { get; set; }
        [Required] public DateTime ExpiryDate { get; set; }
        [Required] public bool Valid { get; set; }
        [Required] public string Purpose { get; set; }
        [Required] public bool Used { get; set; }
        public DateTime? UsedOn { get; set; }
    }
}
