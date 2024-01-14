using AcePacific.Busines.Services;
using AcePacific.Common.Constants;
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

        [HttpPost("ValidatePin")]
        public async Task<ActionResult<Response<string>>> ValidatePin(ValidatePinModel model)
        {
            try
            {
                var response = await _walletSevice.ValidatePin(model);
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
        [HttpPost("MakeInterTransfer")]
        public async Task<ActionResult<Response<string>>> MakeInterTransfer(InterTransferDto model)
        {
            try
            {
                var response = await _walletSevice.InterTransfer(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("ResetPin")]
        public async Task<ActionResult<Response<string>>> ResetPin(UpdatePinModel model)
        {
            try
            {
                var response = await _walletSevice.ResetPin(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("CreatePin")]
        public async Task<ActionResult<Response<string>>> CreatePin(ValidatePinModel model)
        {
            try
            {
                var response = await _walletSevice.CreatePin(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("getpaged/{page:int:min(1)}/{pagesize:int:min(1)}/{whereCondition}")]
        public async Task<ActionResult<Response<CountModel<WalletItem>>>> GetCount(int page, int pagesize, string whereCondition)
        {
            try
            {
                var filter = WalletFilter.Deserialize(whereCondition);
                var response = await _walletSevice.GetCount(page, pagesize, filter);
                return Ok(response);
            }
            catch (Exception ec)
            {
                return BadRequest(Response<CountModel<WalletItem>>.Failed(ErrorMessages.GenericError + " : " + ec.Message));
            }
        }

        [HttpGet("querypaged/{page:int:min(1)}/{pagesize:int:min(1)}/{whereCondition}")]
        public async Task<ActionResult<Response<IEnumerable<WalletItem>>>> GetQuery(int page, int pagesize, string whereCondition)
        {
            try
            {
                var filter = WalletFilter.Deserialize(whereCondition);
                var response = await _walletSevice.Query(page, pagesize, filter);
                return Ok(response);
            }
            catch (Exception ec)
            {
                return BadRequest(Response<IEnumerable<WalletItem>>.Failed(ErrorMessages.GenericError + " : " + ec.Message));
            }
        }

        [HttpGet("ViewUserTransactionHistory")]
        public async Task<ActionResult<Response<IEnumerable<TransactionHistoryView>>>> ViewUserTransactionHistory(string userId)
        {
            try
            {
                var response = await _walletSevice.ViewUserTransactionHistory(userId);
                return Ok(response);
            }
            catch(Exception e)
            {
                return BadRequest();
            }
        }
        [HttpGet("GettransactionByReference")]
        public async Task<ActionResult<Response<IEnumerable<TransactionHistoryView>>>> GettransactionByReference(string reference)
        {
            try
            {
                var response = await _walletSevice.GettransactionByReference(reference);
                return Ok(response);
            }
            catch(Exception e)
            {
                return BadRequest();
            }
        }
    }
}
