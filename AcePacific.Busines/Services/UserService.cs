﻿using AcePacific.Common.Constants;
using AcePacific.Common.Contract;
using AcePacific.Common.Helpers;
using AcePacific.Data.Entities;
using AcePacific.Data.Migrations;
using AcePacific.Data.Repositories;
using AcePacific.Data.ViewModel;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AcePacific.Busines.Services
{
    public interface IUserService
    {
        //Task<Response<string>> ChangePassword(string userId, ChangePassword model);
        Task<Response<PhoneNumberExistsDto>> CheckPhoneNumberExists(string phoneNumber);
        Task<Response<CountModel<CustomerItem>>> Count(int page, int pagesize, CustomerFilter filter);
        Task<Response<CustomerModel>> GetUserById(string userId);
        Task<Response<LoginItem>> Login(LoginDto model);
        Task<Response<IEnumerable<CustomerItem>>> Query(int page, int pagesize, CustomerFilter filter);
        Task<Response<CustomerViewItem>> RegisterUser(RegisterUserModel model);
        Task<Response<UpdateUserView>> UpdateUser(string id, UpdateUserModel model);
        Task<Response<string>> UploadUserImage(string userId, IFormFile imageFile);
        Task<Response<string>> UploadUserImageCloudinary(string userId, IFormFile imageFile);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IWalletRepository _walletRepository;
        private readonly IMailService _mailService;
        private readonly IImageAccessor _imageAccessor;
        public UserService(IUserRepository userRepository, UserManager<User> userManager, SignInManager<User> signInManager,
            IMapper mapper, ITokenService tokenService,
            IWalletRepository walletReposiroty, IMailService mailService, IImageAccessor imageAccessor)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _tokenService = tokenService;
            _walletRepository = walletReposiroty;
            _mailService = mailService;
            _imageAccessor = imageAccessor;
        }
        public async Task<Response<PhoneNumberExistsDto>> CheckPhoneNumberExists(string phoneNumber)
        {
            var response = Response<PhoneNumberExistsDto>.Failed(string.Empty);
            try
            {
                var existingCustomer = _userRepository.FindByPhoneNumber(phoneNumber);
                if (existingCustomer != null)
                {
                    response = Response<PhoneNumberExistsDto>.Success(new PhoneNumberExistsDto
                    {
                        PhoneNumber = phoneNumber,
                        CustomerName = $"{existingCustomer.FirstName} {existingCustomer.LastName}"
                    });
                }
                else
                {
                    response = Response<PhoneNumberExistsDto>.Failed(ErrorMessages.UserNotFound);
                }
            }
            catch (Exception ex)
            {
                response = Response<PhoneNumberExistsDto>.Failed(ex.Message);
            }
            return await Task.FromResult(response);
        }
        public async Task<Response<LoginItem>> Login(LoginDto model)
        {
            var response = Response<LoginItem>.Failed(string.Empty);
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return Response<LoginItem>.Failed(ErrorMessages.UserNotFound);
                if(user.IsBlocked == true)
                {
                    return Response<LoginItem>.Failed("This account has been blocked from transactions. Kindly email us at Contact@acepacificdigital.com for immediate assistance");
                }
                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (!result.Succeeded)
                    return Response<LoginItem>.Failed(ErrorMessages.UserNameOrPasswordIncorrect);

                if(user.ProfilePictureUrl == null)
                {
                    user.ProfilePictureUrl = "";
                }
                response = Response<LoginItem>.Success(new LoginItem
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Id = user.Id,
                    AccountNumber = user.AccountNumber,
                    Initial = Helper.ComputeInitials(user.FirstName, user.LastName),
                    Token = _tokenService.CreateToken(user),
                    UserId = user.UserId,
                    ProfilePictureUrl = user.ProfilePictureUrl
                });
            }
            catch (Exception ex)
            {
                response = Response<LoginItem>.Failed(ex.Message);
            }
            return await Task.FromResult(response);
        }
        public async Task<Response<CustomerModel>> GetUserById(string userId)
        {
            var response = Response<CustomerModel>.Failed(string.Empty);
            try
            {
                var entity = _userRepository.FindUserById(userId);

                var mappedEntity = _mapper.Map<CustomerModel>(entity);
                if (mappedEntity.ProfilePicture == null)
                {
                    mappedEntity.UserInitials = Helper.ComputeInitials(mappedEntity.FirstName, mappedEntity.LastName);
                }
                response = Response<CustomerModel>.Success(mappedEntity);

            }
            catch (Exception ex)
            {
                response = Response<CustomerModel>.Failed(ErrorMessages.GenericError + ex.Message);
            }
            return await Task.FromResult(response);
        }

        public async Task<Response<CustomerViewItem>> RegisterUser(RegisterUserModel model)
        {
            var response = Response<CustomerViewItem>.Failed(string.Empty);
            try
            {
                bool isEmail = Regex.IsMatch(model.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                if (!isEmail)
                    return Response<CustomerViewItem>.Failed(ErrorMessages.InvalidEmail);
                var userEmailExists = _userRepository.EmailExiststs(model.Email);
                //var phoneExists = _userRepository.PhoneNumberExists(model.PhoneNumber);
                var userName = _userRepository.UserNameExists(model.UserName);

                if (userEmailExists)
                    return Response<CustomerViewItem>.Failed(ErrorMessages.UserEmailAlreadyExists);
                /*if (phoneExists)
                    return Response<CustomerViewItem>.Failed(ErrorMessages.phoneNumberExists);*/
                if (userName)
                    return Response<CustomerViewItem>.Failed(ErrorMessages.UserNameExists);

                if (model.Password != model.ConfirmPassword)
                    return Response<CustomerViewItem>.Failed(ErrorMessages.PasswordMismatchError);

                var userAccountNumber = Helper.GenerateRandomAccountNumber();

                var checkAccountNumberExists = _userRepository.AccountNumberExists(userAccountNumber);

                if (checkAccountNumberExists)
                    return Response<CustomerViewItem>.Failed(ErrorMessages.UserCreationFailed);

                EmailTemplate emailTemplate = new()
                {
                    AccountNumber = "9034343343",
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email
                };

                await _mailService.SendWelcomeEmailAsync(emailTemplate);



                var user = new RegisterUserModel
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Password,
                    ConfirmPassword = model.ConfirmPassword,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.UserName,
                    Gender = model.Gender,
                };

                var mappedUser = _mapper.Map<User>(user);
                mappedUser.AccountNumber = userAccountNumber;
                mappedUser.IsActive = true;
                mappedUser.IsTransactionPinSet = false;
                mappedUser.UserType = Common.Enums.UserType.User;

                var registeredUser = await _userManager.CreateAsync(mappedUser, model.Password);
                var newWallet = new CreatWalletViewModel
                {
                    UserId = mappedUser.Id,
                    WalletAccountNumber = userAccountNumber,
                    WalletBalance = 0

                };

                var mappedWallet = _mapper.Map<Wallet>(newWallet);

                await _walletRepository.InsertAsync(mappedWallet);

                if (registeredUser.Succeeded)
                {
                    response = Response<CustomerViewItem>.Success(new CustomerViewItem
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        Token = _tokenService.CreateToken(user)
                    });

                }
                else
                {
                    response = Response<CustomerViewItem>.Failed(ErrorMessages.FailedToRegisterUser);
                }
            }
            catch (Exception ex)
            {
                response = Response<CustomerViewItem>.Failed(ex.Message);
            }
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        /*public async Task<Response<string>> ChangePassword(string userId, ChangePassword model)
        {
            var response = Response<string>.Failed(string.Empty);
            try
            {
                var entity = _userRepository.Table.AsNoTracking().FirstOrDefaultAsync(c => c.Id == userId);
                if (entity == null)
                    return Response<string>.Failed($"No user found");
                var oldHashedPassword = _userManager.CheckPasswordAsync(entity, model.OldPassword);
                
                if (!oldHashedPassword.IsCompleted)
                    return Response<string>.Failed("Old password do not match with the inputted passsword");

                if (model.NewPassword != model.ConfirmPassword)
                    return Response<string>.Failed("Passwords do not match");

                var changedPassword = await _userManager.ChangePasswordAsync(entity, model.OldPassword, model.NewPassword);
                if (changedPassword.Succeeded)
                    response = Response<string>.Success("Password changed successfully");

            }catch(Exception ex)
            {
                response = Response<string>.Failed(ex.Message);
            }
            return await Task.FromResult(response);

        }*/
        public async Task<Response<UpdateUserView>> UpdateUser(string id, UpdateUserModel model)
        {
            var response = Response<UpdateUserView>.Failed(string.Empty);
            var userView = new UpdateUserView();
            try
            {
                var entityExists = await _userRepository.Table.FirstOrDefaultAsync(c => c.Id == id);
                if (entityExists == null)
                    return Response<UpdateUserView>.Failed(ErrorMessages.UserNotFound);

                entityExists.LastUpdatedOn = DateTime.Now.ToUniversalTime();
                entityExists.LastUpdatedBy = entityExists.FirstName;

                if (await _userRepository.Table.AnyAsync(c => c.Id != id && c.UserName == model.UserName))
                    return Response<UpdateUserView>.Failed(ErrorMessages.UserNameExists);

                if (await _userRepository.Table.AnyAsync(c => c.Id != id && c.PhoneNumber == model.PhoneNumber))
                    return Response<UpdateUserView>.Failed(ErrorMessages.phoneNumberExists);

                await _userRepository.UpdateAsync(entityExists, true);
                var mappedUser = _mapper.Map(entityExists, userView);
                response = Response<UpdateUserView>.Success(mappedUser);
            }
            catch (Exception ex)
            {
                response = Response<UpdateUserView>.Failed(ex.Message);
            }
            return await Task.FromResult(response);
        }
        public async Task<Response<IEnumerable<CustomerItem>>> Query(int page, int pagesize, CustomerFilter filter)
        {
            var response = Response<IEnumerable<CustomerItem>>.Failed(string.Empty);
            try
            {
                int totalCount;
                var orderBy = OrderExpression.Deserilizer("{}");
                var entities = _userRepository.GetCustomerPaged(page, pagesize, out totalCount, filter, orderBy);
                response = Response<IEnumerable<CustomerItem>>.Success(ProcessQuery(entities));
            }
            catch (Exception)
            {
                response = Response<IEnumerable<CustomerItem>>.Failed(ErrorMessages.GenericError);
            }
            return response;
        }
        public async Task<Response<CountModel<CustomerItem>>> Count(int page, int pagesize, CustomerFilter filter)
        {
            var response = Response<CountModel<CustomerItem>>.Failed(string.Empty);
            try
            {
                int totalCount;
                var orderBy = OrderExpression.Deserilizer("{}");
                var entities = _userRepository.GetCustomerPaged(page, pagesize, out totalCount, filter, orderBy);

                var result = Response<CountModel<CustomerItem>>.Success(new CountModel<CustomerItem>()
                {
                    Total = totalCount,
                    Items = ProcessQuery(entities)
                });
                return await Task.FromResult(result);
            }
            catch (Exception)
            {
                response = Response<CountModel<CustomerItem>>.Failed(ErrorMessages.GenericError);
            }
            return response;
        }
        /*public async Task<Response<string>> UploadUserImage(string userId, IFormFile imageFile)
        {
            var response = Response<string>.Failed(string.Empty);
            try
            {
                var user = await GetUserById(userId);
                if (user == null)
                    return Response<string>.Failed(ErrorMessages.UserDoesntExist);
                if(user.Successful)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(memoryStream);
                        user.Result.ProfilePicture = memoryStream.ToArray();
                    }
                    var mappedUser = _mapper.Map<User>(user);
                     await _userRepository.UpdateAsync(mappedUser);

                }
                response = Response<string>.Success("User Image Updated");
            }catch(Exception ex)
            {
                response = Response<string>.Failed(ErrorMessages.GenericError + ex.Message);
            }
            return await Task.FromResult(response);
        }*/
        public async Task<Response<string>> UploadUserImage(string userId, IFormFile imageFile)
        {
            var response = Response<string>.Failed(string.Empty);
            try
            {
                var user = _userRepository.FindUserById(userId);
                if (user == null)
                    return Response<string>.Failed(ErrorMessages.UserDoesntExist);

                if (user != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(memoryStream);
                        user.ProfilePicture = memoryStream.ToArray();
                    }

                }
                await _userRepository.UpdateAsync(user);

                // Save the updated user object to the database
                //await _userRepository.SaveAsync(mappedUser);

                response = Response<string>.Success("User Image Updated");
            }
            catch (Exception ex)
            {
                response = Response<string>.Failed(ex.Message);
            }

            return await Task.FromResult(response);
        }
        public async Task<Response<string>> UploadUserImageCloudinary(string userId, IFormFile imageFile)
        {
            var response = Response<string>.Failed(string.Empty);
            try
            {
                var user = _userRepository.FindUserById(userId);
                if (user == null)
                    return Response<string>.Failed(ErrorMessages.UserDoesntExist);

                var imageUrl = await _imageAccessor.UploadImageAsync(imageFile);

                user.ProfilePictureUrl = imageUrl.Result.ImageUrl;
                await _userRepository.UpdateAsync(user, true);
                response = Response<string>.Success("Image uploaded successfully");
            }
            catch (Exception ex)
            {
                response = Response<string>.Failed(ex.Message);
            }
            return await Task.FromResult(response);
        }

        private IEnumerable<CustomerItem> ProcessQuery(IEnumerable<User> entities)
        {
            return entities.ToList().Select(c =>
            {
                var item = _mapper.Map<CustomerItem>(c);
                return item;
            }).ToList();
        }
    }
}
