﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AcePacific.Data.ViewModel
{
    internal class BankViewModel
    {
    }
    public class CustomerModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
        public string HouseNumber { get; set; }
        public string Street { get; set; }
        public string LandMark { get; set; }
        public string AddressLine1 { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string AddressLine2 { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string UserInitials { get; set; }
    }
    public class CustomerItem : CustomerModel
    {
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public DateTime? VerifiedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime DateCreated { get; set; }
    }
    public class CustomerFilter : CustomerItem
    {
        public string Name { get; set; }
        public DateTime? DateCreatedFrom { get; set; }
        public DateTime? DateCreatedTo { get; set; }
        public static CustomerFilter Deserialize(string whereCondition)
        {
            var filter = new CustomerFilter();
            if (!string.IsNullOrEmpty(whereCondition))
            {
                filter = JsonConvert.DeserializeObject<CustomerFilter>(whereCondition);
            }
            return filter;
        }
    }
    public class PhoneNumberExistsDto
    {
        public string PhoneNumber { get; set; }
        public string CustomerName { get; set; }
    }
    public class RegisterUserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        //[PasswordPropertyText]
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
    }
    public class CustomerViewItem
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
    }
    public class LoginItem
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Id { get; set; }
        public string AccountNumber { get; set; }
        public string Token { get; set; }
        public string Initial { get; set; }
        public string UserId { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class CurrentUserItem
    {
        public string Email { get; set; }
    }
    public class UpdateUserView
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class UpdateUserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
        public string? HouseNumber { get; set; }
        public string? Street { get; set; }
        public string? LandMark { get; set; }
        public string? AddressLine1 { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? AddressLine2 { get; set; }
    }
    public class EmailTemplate
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountNumber { get; set; }
        public string UserName { get; set; }
        public string Email  { get; set; }
    }
    public class ChangePassword
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
