namespace AcePacific.Data.Entities
{
    public class Bank
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public DateTime DateUpdated { get; set; }

    }
}
