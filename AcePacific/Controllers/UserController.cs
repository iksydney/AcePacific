using AcePacific.API.ExtensionServices;
using AcePacific.Busines.Services;
using AcePacific.Common.Constants;
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
        
        /*[HttpPost("ChangePassword")]
        public async Task<ActionResult<Response<LoginItem>>> ChangePassword(string userId, ChangePassword model)
        {
            try
            {
                var response = await _userService.ChangePassword(userId, model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }*/

        [HttpPost("UpdateProfilePicture")]
        public async Task<ActionResult<Response<string>>> UpdateProfileImage(string userId, IFormFile imageFile)
        {
            try
            {
                var response = await _userService.UploadUserImage(userId, imageFile);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetUserById")]
        public async Task<ActionResult<Response<CustomerModel>>> GetUserById(string userId)
        {
            try
            {
                var response = await _userService.GetUserById(userId);
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
            if(user == null)
                return NotFound("Current User not found");
            return new LoginItem
            {
                Email = user.Email,
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }
        [HttpGet("getpaged/{page:int:min(1)}/{pagesize:int:min(1)}/{whereCondition}")]
        public async Task<ActionResult<Response<CountModel<CustomerItem>>>> Count(int page = 1, int pagesize = 10, string whereCondition = "{}")
        {
            try
            {
                var filter = CustomerFilter.Deserialize(whereCondition);
                var response = await _userService.Count(page, pagesize, filter);

                return Ok(response);
            }
            catch (Exception)
            {
                return BadRequest(Response<CountModel<CustomerItem>>.Failed(ErrorMessages.GenericError));
            }

        }
        [HttpGet("querypaged/{page:int:min(1)}/{pagesize:int:min(1)}/{whereCondition}")]
        public async Task<ActionResult<Response<IEnumerable<CustomerItem>>>> Query(int page = 1, int pagesize = 10, string whereCondition = "{}")
        {
            try
            {
                var filter = CustomerFilter.Deserialize(whereCondition);
                var response = await _userService.Query(page, pagesize, filter);

                return Ok(response);
            }
            catch (Exception)
            {
                return BadRequest(Response<IEnumerable<CustomerItem>>.Failed(ErrorMessages.GenericError));
            }
        }

        [HttpPut("UpdateUser")]
        public async Task<ActionResult<Response<UpdateUserView>>> UpdateUser(string id, UpdateUserModel model)
        {
            try
            {
                var result = await _userService.UpdateUser(id, model);
                return Ok(result);
            }catch(Exception ex)
            {
                return BadRequest(Response<IEnumerable<CustomerItem>>.Failed(ErrorMessages.GenericError));
            }
        }
    }
}
