using AcePacific.Common.Enums;

namespace AcePacific.Common.Contract
{
    public class UserProfileModel
    {
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Client { get; set; }
        public UserType UserType { get; set; }
        public string UserId { get; set; }
        public string? IsFirstLogin { get; set; }
    }
}
