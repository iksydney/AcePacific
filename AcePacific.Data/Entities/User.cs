using AcePacific.Common.Enums;
using Microsoft.AspNetCore.Identity;

namespace AcePacific.Data.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
        public UserType UserType { get; set; } = UserType.User;
        public bool IsActive { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string CreatedBy
        {
            get
            {
                return $"{UserName}";
            }
        }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateOfBirth { get; set; }
        public DateTime? AddressVerificationDate { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? LastTransactedOn { get; set; }
        public string? HouseNumber { get; set; }
        public string? Street { get; set; }
        public string? LandMark { get; set; }
        public string? AddressLine1 { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? AddressLine2 { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
        public bool? ChangePin { get; set; }
        public string? TransactionPin { get; set; }
        public bool? IsTransactionPinSet { get; set; }
        public string? IdentificationType { get; set; }
        public string? IdentificationDocumentUrl { get; set; }
        public string UserId
        {
            get
            {
                return $"{Id}";
            }
        }
        public string Gender { get; set; }
    }
}
