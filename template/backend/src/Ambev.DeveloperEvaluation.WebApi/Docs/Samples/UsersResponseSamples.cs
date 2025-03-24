using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;
using Swashbuckle.AspNetCore.Filters;
using Tynamix.ObjectFiller;

namespace Ambev.DeveloperEvaluation.WebApi.Docs.Samples
{
    public static class UserSamplesUtils
    {
    }

    public class UserRequestSamples
    {
        public class CreateUserRequestSample : IExamplesProvider<CreateUserRequest>
        {
            public CreateUserRequest GetExamples()
            {
                return new Filler<CreateUserRequest>().Create();
            }
        }
    }

    public class UserResponseSamples
    {
        public class CreateUserResponseSample : IExamplesProvider<ApiResponseWithData<CreateUserResponse>>
        {
            public ApiResponseWithData<CreateUserResponse> GetExamples()
            {
                return new ApiResponseWithData<CreateUserResponse>
                {
                    Success = true,
                    Message = "User created successfully",
                    Data = new Filler<CreateUserResponse>().Create()
                };
            }
        }

        public class GetUserResponseSample : IExamplesProvider<ApiResponseWithData<GetUserResponse>>
        {
            public ApiResponseWithData<GetUserResponse> GetExamples()
            {
                return new ApiResponseWithData<GetUserResponse>
                {
                    Success = true,
                    Message = "User retrieved successfully",
                    Data = new Filler<GetUserResponse>().Create()
                };
            }
        }

        public class GetUsersListResponseSample : IExamplesProvider<PaginatedResponse<GetUserResponse>>
        {
            public PaginatedResponse<GetUserResponse> GetExamples()
            {
                var data = new Filler<IEnumerable<GetUserResponse>>().Create().Take(5);
                
                return
                    new PaginatedResponse<GetUserResponse>
                    {
                        Success = true,
                        Message = "Users retrieved successfully",
                        Data = data,
                        CurrentPage = 1,
                        TotalPages = (int)(data.Count()/5),
                        TotalCount = data.Count()
                    };
            }
        }

        public class DeleteUserResponseSample : IExamplesProvider<ApiResponse>
        {
            public ApiResponse GetExamples()
            {
                return new ApiResponse
                {
                    Success = true,
                    Message = "User deleted successfully",
                    Errors = new List<ValidationErrorDetail>()
                };
            }
        }
    }

    public class UserErrorSamples
    {
        public class NotFoundErrorSample : IExamplesProvider<ApiResponse>
        {
            public ApiResponse GetExamples()
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "User not found",
                    Errors = new List<ValidationErrorDetail>
                    {
                        new ValidationErrorDetail { Error = "404", Detail = "User with the specified ID does not exist" }
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
                        new ValidationErrorDetail { Error = "400", Detail = "Invalid User data provided" }
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
