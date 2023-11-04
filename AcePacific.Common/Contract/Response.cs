using AcePacific.Common.Enums;
using AcePacific.Common.Extensions;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace AcePacific.Common.Contract
{
    public class Response<T> where T : class
    {
        public bool HasResult
        {
            get
            {
                return Result != null;
            }
        }
        public T Result { get; set; }
        public ResultType ResultType { get; set; }
        public string Message { get; set; }
        public ImmutableList<string> ValidationMessages { get; set; }
        public bool Successful
        {
            get
            {
                return ResultType == ResultType.Success || ResultType == ResultType.Warning;
            }
        }
        public static Response<T> Create(T result)
        {
            var response = new Response<T>
            {
                ResultType = ResultType.Success,
                Message = ResultType.Success.GetDescription(),
                ValidationMessages = new List<string>().ToImmutableList(),
                Result = result
            };

            return response;
        }
        public static Response<T> Success(T result, string message = "Successful")
        {
            var response = new Response<T> { ResultType = ResultType.Success, Result = result, Message = message };

            return response;
        }
        public static Response<T> Failed(string errorMessage)
        {
            var response = new Response<T> { ResultType = ResultType.Error, Message = errorMessage };

            return response;
        }
        public static Response<T> ValidationError(ImmutableList<string> validationMessages)
        {
            var response = new Response<T>
            {
                ResultType = ResultType.ValidationError,
                Message = validationMessages?.FirstOrDefault() ?? "Response has validation errors",
                ValidationMessages = validationMessages
            };

            return response;
        }
        public static Response<T> ValidationError(IEnumerable<ValidationResult> validationMessages)
        {
            var response = new Response<T>
            {
                ResultType = ResultType.ValidationError,
                Message = validationMessages?.FirstOrDefault()?.ErrorMessage ?? "Response has validation errors",
                ValidationMessages = validationMessages.Select(error => error.ErrorMessage).ToImmutableList()
            };

            return response;
        }
        public static Response<T> Warning(string warningMessage, T result)
        {
            var response = new Response<T>
            {
                ResultType = ResultType.Warning,
                Message = warningMessage,
                Result = result
            };

            return response;
        }
        public static Response<T> Empty()
        {
            var response = new Response<T> { ResultType = ResultType.Empty };

            return response;
        }
    }
    public class Response
    {
        public Response()
        {
            this.ResultType = ResultType.Success;
        }
        public virtual bool HasResult
        {
            get
            {
                return false;
            }
        }
        public object Result { get; protected set; }
        public bool Successful
        {
            get
            {
                return this.ResultType == ResultType.Success || this.ResultType == ResultType.Warning;
            }
        }
        public ResultType ResultType { get; set; }
        public string Message { get; set; }
        public List<string> ValidationMessages { get; set; }
        public static Response Success()
        {
            var response = new Response { ResultType = ResultType.Success };

            return response;
        }
        public static Response Failed(string errorMessage)
        {
            var response = new Response { ResultType = ResultType.Error, Message = errorMessage };

            return response;
        }
        public static Response ValidationError(List<string> validationMessages)
        {
            var response = new Response { ResultType = ResultType.ValidationError, ValidationMessages = validationMessages };

            return response;
        }
        public static Response Warning(string warningMessage)
        {
            var response = new Response { ResultType = ResultType.Warning, Message = warningMessage };

            return response;
        }
        public static Response CustomerInformation(string customerInformationMessage)
        {
            var response = new Response
            {
                ResultType = ResultType.CustomerInformation,
                Message = customerInformationMessage
            };

            return response;
        }
        public static Response Empty()
        {
            var response = new Response { ResultType = ResultType.Empty };

            return response;
        }
    }
}
