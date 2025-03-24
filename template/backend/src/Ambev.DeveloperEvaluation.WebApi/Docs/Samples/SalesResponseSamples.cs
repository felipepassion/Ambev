using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Swashbuckle.AspNetCore.Filters;
using Tynamix.ObjectFiller;

namespace Ambev.DeveloperEvaluation.WebApi.Docs.Samples
{
    public static class SaleSamplesUtils
    {
    }

    public class SaleRequestSamples
    {
        public class CreateSaleRequestSample : IExamplesProvider<CreateSaleRequest>
        {
            public CreateSaleRequest GetExamples()
            {
                return new Filler<CreateSaleRequest>().Create();
            }
        }
    }


    public class SaleResponseSamples
    {
        public class CreateSaleResponseSample : IExamplesProvider<ApiResponseWithData<CreateSaleResponse>>
        {
            public ApiResponseWithData<CreateSaleResponse> GetExamples()
            {
                return new ApiResponseWithData<CreateSaleResponse>
                {
                    Success = true,
                    Message = "Sale created successfully",
                    Data = new Filler<CreateSaleResponse>().Create()
                };
            }
        }

        public class GetSaleResponseSample : IExamplesProvider<ApiResponseWithData<GetSaleResponse>>
        {
            public ApiResponseWithData<GetSaleResponse> GetExamples()
            {
                var result = new Filler<GetSaleResponse>().Create();
                result.Items = result.Items!.Take(3).ToList();
                return new ApiResponseWithData<GetSaleResponse>
                {
                    Success = true,
                    Message = "Sale retrieved successfully",
                    Data = result
                };
            }
        }

        public class GetSalesListResponseSample : IExamplesProvider<PaginatedResponse<GetSaleResponse>>
        {
            public PaginatedResponse<GetSaleResponse> GetExamples()
            {
                var data = new Filler<IEnumerable<GetSaleResponse>>().Create().Take(5);

                return
                    new PaginatedResponse<GetSaleResponse>
                    {
                        Success = true,
                        Message = "Sales retrieved successfully",
                        Data = data,
                        CurrentPage = 1,
                        TotalPages = (int)(data.Count() / 5),
                        TotalCount = data.Count()
                    };
            }
        }

        public class DeleteSaleResponseSample : IExamplesProvider<ApiResponse>
        {
            public ApiResponse GetExamples()
            {
                return new ApiResponse
                {
                    Success = true,
                    Message = "Sale deleted successfully",
                    Errors = new List<ValidationErrorDetail>()
                };
            }
        }

        public class CancelSaleResponseSample : IExamplesProvider<ApiResponse>
        {
            public ApiResponse GetExamples()
            {
                return new ApiResponse
                {
                    Success = true,
                    Message = "Sale canceld successfully",
                    Errors = new List<ValidationErrorDetail>()
                };
            }
        }
    }

    public class SaleErrorSamples
    {
        public class NotFoundErrorSample : IExamplesProvider<ApiResponse>
        {
            public ApiResponse GetExamples()
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Sale not found",
                    Errors = new List<ValidationErrorDetail>
                    {
                        new ValidationErrorDetail { Error = "404", Detail = "Sale with the specified ID does not exist" }
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
                        new ValidationErrorDetail { Error = "400", Detail = "Invalid Sale data provided" }
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
