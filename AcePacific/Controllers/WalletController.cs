using AcePacific.Busines.Services;
using AcePacific.Common.Contract;
using AcePacific.Data.Repositories;
using AcePacific.Data.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace AcePacific.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : BaseApiController
    {
        private readonly IWalletService _walletSevice;
        public WalletController(IWalletService walletService)
        {
            _walletSevice = walletService;
        }
        [HttpGet("GetUserAccountAdmin/{accountNumber}")]
        public async Task<ActionResult<Response<PhoneNumberExistsDto>>> GetUserAccountAdmin(string accountNumber)
        {
            try
            {
                var response = await _walletSevice.GetWalletAdmin(accountNumber);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetUserAccount/{accountNumber}")]
        public async Task<ActionResult<Response<PhoneNumberExistsDto>>> GetUserAccount(string accountNumber)
        {
            try
            {
                var response = await _walletSevice.GetWallet(accountNumber);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("MakeIntraTransfer")]
        public async Task<ActionResult<Response<string>>> MakeIntraTransfer(IntraTransferDto model)
        {
            try
            {
                var response = await _walletSevice.IntraTransfer(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("UpdatePin")]
        public async Task<ActionResult<Response<string>>> UpdatePin(UpdatePinModel model)
        {
            try
            {
                var response = await _walletSevice.UpdatePin(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
