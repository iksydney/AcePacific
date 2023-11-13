using AcePacific.API.ExtensionServices;
using AcePacific.Busines.Services;
using AcePacific.Common.Contract;
using AcePacific.Data.Entities;
using AcePacific.Data.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AcePacific.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        public UserController(IUserService userService, UserManager<User> userManager, ITokenService tokenService)
        {
            _userService = userService;
            _userManager = userManager;
            _tokenService = tokenService;
        }
        [HttpGet("GetUserByPhoneNumber/{phoneNumber}")]
        public async Task<ActionResult<Response<PhoneNumberExistsDto>>> CheckifPhoneNumberExists(string phoneNumber)
        {
            try
            {
                var response = await _userService.CheckPhoneNumberExists(phoneNumber);
                return Ok(response);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("RegisterUser")]
        public async Task<ActionResult<Response<CustomerViewItem>>> RegisterUser(RegisterUserModel model)
        {
            try
            {
                var response = await _userService.RegisterUser(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("Login")]
        public async Task<ActionResult<Response<LoginItem>>> Login(LoginDto model)
        {
            try
            {
                var response = await _userService.Login(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<LoginItem>> GetCurrentUser()
        {
            var user = await _userManager.FindByEMailFromClaimPrincipal(User);
            return new LoginItem
            {
                Email = user.Email,
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

    }
}
