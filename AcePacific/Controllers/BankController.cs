using AcePacific.Busines.Services;
using AcePacific.Common.Contract;
using AcePacific.Data.ViewModel;
using Microsoft.AspNetCore.Mvc;
using static AcePacific.Data.ViewModel.BankFilter;

namespace AcePacific.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : BaseApiController
    {
        private readonly IBankService _bankService;
        public BankController(IBankService bankService)
        {
            _bankService = bankService;
        }
        [HttpPost("CreateBank")]
        public async Task<ActionResult<Response<BankModel>>> CreateBank(CreateBank bank)
        {
            try
            {
                var response = await _bankService.CreateBank(bank);
                return Ok(response);
            }catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet("GetBankById/{id}")]
        public async Task<ActionResult<Response<BankModel>>> GetBankById(int id)
        {
            try
            {
                var response = await _bankService.GetBankById(id);
                return Ok(response);
            }catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
