using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Swashbuckle.AspNetCore.Filters;
using Tynamix.ObjectFiller;

namespace Ambev.DeveloperEvaluation.WebApi.Docs.Samples
{
    public static class AuthSamplesUtils
    {
    }

    public class AuthResponseSamples
    {
        public class CreateAuthResponseSample : IExamplesProvider<ApiResponseWithData<AuthenticateUserResult>>
        {
            public ApiResponseWithData<AuthenticateUserResult> GetExamples()
            {
                var result = new Filler<AuthenticateUserResult>().Create();
                result.Token = "uFQCfZq5kPqBSyIDOcwo3KT2grmUzvHCV1jWCsrv1mjz5Jeog697kLyeJkP8CGO2tqArbzL7w5fgBZbBezGJO9XRml2ww6vPWq4FyyRSfr4MIICso78kZsdr3ToIwHYtpbbTiy6QIxRYIGdkBlXb41GE";
                return new ApiResponseWithData<AuthenticateUserResult>
                {
                    Success = true,
                    Message = "Auth created successfully",
                    Data = result
                };
            }
        }
    }

    public class AuthErrorSamples
    {
        public class NotFoundErrorSample : IExamplesProvider<ApiResponse>
        {
            public ApiResponse GetExamples()
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Auth not found",
                    Errors = new List<ValidationErrorDetail>
                    {
                        new ValidationErrorDetail { Error = "404", Detail = "Auth with the specified ID does not exist" }
                    }
                };
            }
        }

        public class BadRequestErrorSample : IExamplesProvider<ApiResponse>
        {
            public ApiResponse GetExamples()
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Bad request",
                    Errors = new List<ValidationErrorDetail>
                    {
                        new ValidationErrorDetail { Error = "400", Detail = "Invalid Auth data provided" }
                    }
                };
            }
        }

        public class InternalServerErrorSample : IExamplesProvider<ApiResponse>
        {
            public ApiResponse GetExamples()
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Internal server error",
                    Errors = new List<ValidationErrorDetail>
                    {
                        new ValidationErrorDetail { Error = "500", Detail = "An unexpected error occurred while processing the request." }
                    }
                };
            }
        }
    }
}
