using AcePacific.API.Filter;
using AcePacific.Common.Contract;
using AcePacific.Common.EnumHelper;
using AcePacific.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AcePacific.API.Controllers
{
    [ValidateModel]
    public class BaseApiController : ControllerBase
    {
        protected string GetUserName()
        {
            return GetUserProfile().UserName;
        }
        protected string GetClientId()
        {
            return GetUserProfile().Client;
        }

        protected string GetUserId()
        {
            return GetUserProfile().UserId;
        }
        protected UserProfileModel GetUserProfile()
        {
            var profile = new UserProfileModel();
            var claims = User.Identity as ClaimsIdentity;
            var userClaim = claims.FindFirst("phonenumber");
            if (userClaim != null)
            {
                profile.UserName = userClaim.Value;
            }
            var userTypeClaim = claims.FindFirst("usertypeid");
            var userType = string.Empty;
            if (userTypeClaim != null)
            {
                profile.UserType = EnumHelper.ParseEnum<UserType>(userTypeClaim.Value);
            }

            var userownerClaim = claims.FindFirst("userid");
            var userowner = string.Empty;
            if (userownerClaim != null)
            {
                profile.UserId = userownerClaim.Value;
            }

            var clientClaim = claims.FindFirst("client_id");
            var client = string.Empty;
            if (clientClaim != null)
            {
                profile.Client = clientClaim.Value;
            }
            var userName = claims.FindFirst("username");
            if (userName != null)
            {
                profile.UserName = userName.Value;
            }

            var merchantId = claims.FindFirst("merchantid");
            
            var isFirstLogin = claims.FindFirst("IsFirstLogin");
            if (isFirstLogin != null)
            {
                profile.IsFirstLogin = isFirstLogin.Value;
            }
            return profile;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        protected IActionResult ApiResponse<TPayload>(TPayload payload) where TPayload : class
         => Ok(Response<TPayload>.Success(payload));

        [ApiExplorerSettings(IgnoreApi = true)]
        protected IActionResult ApiResponse<TPayload>(IEnumerable<TPayload> payload, PayloadMetaData metaData) where TPayload : class
        {
            CountModel<TPayload> paginatedData = new CountModel<TPayload>
            {
                Total = metaData.TotalCount,
                Items = payload,
                Info = metaData.PageCount
            };

            return Ok(Response<CountModel<TPayload>>.Success(paginatedData));
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        protected IActionResult ApiErrorResponse<TPayload>(IEnumerable<TPayload> payload) where TPayload : ValidationResult
            => BadRequest(Response<IEnumerable<TPayload>>.ValidationError(payload));


        [ApiExplorerSettings(IgnoreApi = true)]
        protected string GetModelStateValidationErrors() =>
                                    string.Join("; ", ModelState.Values
                                    .SelectMany(a => a.Errors)
                                    .Select(e => e.ErrorMessage));

        [ApiExplorerSettings(IgnoreApi = true)]
        protected string GetModelStateValidationError()
        => ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;
        protected ActionResult<Response<T>> ApiResponse<T>(Response<T> response) where T : class
        {
            return Ok(response);
        }
    }
}
