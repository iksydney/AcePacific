using AcePacific.Common.Constants;
using AcePacific.Common.Contract;
using AcePacific.Common.Helpers;
using AcePacific.Data.Entities;
using AcePacific.Data.Repositories;
using AcePacific.Data.ViewModel;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AcePacific.Busines.Services
{
    public interface IUserService
    {
        Task<Response<PhoneNumberExistsDto>> CheckPhoneNumberExists(string phoneNumber);
        Task<Response<LoginItem>> Login(LoginDto model);
        Task<Response<CustomerViewItem>> RegisterUser(RegisterUserModel model);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IWalletRepository _walletRepository;
        public UserService(IUserRepository userRepository, UserManager<User> userManager, SignInManager<User> signInManager, 
            IMapper mapper, ITokenService tokenService,
            IWalletRepository walletReposiroty)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _tokenService = tokenService;
            _walletRepository = walletReposiroty;
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
            }catch(Exception ex)
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
                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (!result.Succeeded)
                    return Response<LoginItem>.Failed(ErrorMessages.UserNameOrPasswordIncorrect);
                response = Response<LoginItem>.Success(new LoginItem
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Token = _tokenService.CreateToken(user)
                });
            }catch(Exception ex)
            {
                response = Response<LoginItem>.Failed(ex.Message);
            }
            return await Task.FromResult(response);
        }
      
        public async Task<Response<CustomerViewItem>> RegisterUser(RegisterUserModel model)
        {
            var response = Response<CustomerViewItem>.Failed(string.Empty);
            try
            {
                var userEmailExists = _userRepository.EmailExiststs(model.Email);
                var phoneExists = _userRepository.PhoneNumberExists(model.PhoneNumber);
                var userName = _userRepository.UserNameExists(model.UserName);

                if (userEmailExists)
                    return Response<CustomerViewItem>.Failed(ErrorMessages.UserEmailAlreadyExists);
                if (phoneExists)
                    return Response<CustomerViewItem>.Failed(ErrorMessages.phoneNumberExists);
                if (userName)
                    return Response<CustomerViewItem>.Failed(ErrorMessages.UserNameExists);

                if(model.Password != model.ConfirmPassword)
                    return Response<CustomerViewItem>.Failed(ErrorMessages.PasswordMismatchError);

                var userAccountNumber = Helper.GenerateRandomAccountNumber();

                var checkAccountNumberExists = _userRepository.AccountNumberExists(userAccountNumber);

                if(checkAccountNumberExists)
                    return Response<CustomerViewItem>.Failed(ErrorMessages.UserCreationFailed);

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
            }catch(Exception ex)
            {
                response = Response<CustomerViewItem>.Failed(ex.Message);
            }
            return await Task.FromResult(response).ConfigureAwait(false);
        }
    }
}
