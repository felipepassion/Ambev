using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches.CreateBranch;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches.GetBranch;
using Swashbuckle.AspNetCore.Filters;
using Tynamix.ObjectFiller;

namespace Ambev.DeveloperEvaluation.WebApi.Docs.Samples
{
    public static class BranchSamplesUtils
    {
    }

    public class BranchRequestSamples
    {
        public class CreateBranchRequestSample : IExamplesProvider<CreateBranchRequest>
        {
            public CreateBranchRequest GetExamples()
            {
                return new Filler<CreateBranchRequest>().Create();
            }
        }
    }

    public class BranchResponseSamples
    {
        public class CreateBranchResponseSample : IExamplesProvider<ApiResponseWithData<CreateBranchResponse>>
        {
            public ApiResponseWithData<CreateBranchResponse> GetExamples()
            {
                return new ApiResponseWithData<CreateBranchResponse>
                {
                    Success = true,
                    Message = "Branch created successfully",
                    Data = new CreateBranchResponse
                    {
                        Id = Guid.NewGuid(),
                        Name = "Main Branch"
                    }
                };
            }
        }

        public class GetBranchResponseSample : IExamplesProvider<ApiResponseWithData<GetBranchResponse>>
        {
            public ApiResponseWithData<GetBranchResponse> GetExamples()
            {
                return new ApiResponseWithData<GetBranchResponse>
                {
                    Success = true,
                    Message = "Branch retrieved successfully",
                    Data = new GetBranchResponse
                    {
                        Id = Guid.NewGuid(),
                        Name = "Main Branch",
                        IsActive = true
                    }
                };
            }
        }

        public class DeleteBranchResponseSample : IExamplesProvider<ApiResponse>
        {
            public ApiResponse GetExamples()
            {
                return new ApiResponse
                {
                    Success = true,
                    Message = "Branch deleted successfully",
                    Errors = new List<ValidationErrorDetail>()
                };
            }
        }
    }

    public class BranchErrorSamples
    {
        public class NotFoundErrorSample : IExamplesProvider<ApiResponse>
        {
            public ApiResponse GetExamples()
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Branch not found",
                    Errors = new List<ValidationErrorDetail>
                    {
                        new ValidationErrorDetail { Error = "404", Detail = "Branch with the specified ID does not exist" }
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
                        new ValidationErrorDetail { Error = "400", Detail = "Invalid branch data provided" }
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
