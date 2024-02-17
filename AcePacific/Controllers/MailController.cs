using AcePacific.Busines.Services;
using AcePacific.Common.Contract;
using AcePacific.Data.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace AcePacific.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMailService _mailService;
        public MailController(IMailService mailService)
        {
            _mailService = mailService;
        }

        [HttpPost("SendMail")]
        public async Task<ActionResult<bool>> SendMail(MailData mailData)
        {
            try
            {
                var response = _mailService.SendMail(mailData);
                return Ok(response);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
