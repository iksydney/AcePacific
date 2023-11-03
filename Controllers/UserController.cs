using AcePacific.Busines.Services;
using AcePacific.Common.Contract;
using AcePacific.Data.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AcePacific.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
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


    }
}
