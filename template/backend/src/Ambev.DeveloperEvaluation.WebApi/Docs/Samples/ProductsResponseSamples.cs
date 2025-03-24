using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct;
using Swashbuckle.AspNetCore.Filters;

namespace Ambev.DeveloperEvaluation.WebApi.Docs.Samples
{
    public static class ProductSamplesUtils
    {
    }

    public class ProductRequestSamples
    {
        public class CreateProductRequestSample : IExamplesProvider<CreateProductRequest>
        {
            public CreateProductRequest GetExamples()
            {
                return new CreateProductRequest
                {
                    Name = "Main Product"
                };
            }
        }
    }

    public class ProductResponseSamples
    {
        public class CreateProductResponseSample : IExamplesProvider<ApiResponseWithData<CreateProductResponse>>
        {
            public ApiResponseWithData<CreateProductResponse> GetExamples()
            {
                return new ApiResponseWithData<CreateProductResponse>
                {
                    Success = true,
                    Message = "Product created successfully",
                    Data = new CreateProductResponse
                    {
                        Id = Guid.NewGuid(),
                        Name = "Main Product"
                    }
                };
            }
        }

        public class GetProductResponseSample : IExamplesProvider<ApiResponseWithData<GetProductResponse>>
        {
            public ApiResponseWithData<GetProductResponse> GetExamples()
            {
                return new ApiResponseWithData<GetProductResponse>
                {
                    Success = true,
                    Message = "Product retrieved successfully",
                    Data = new GetProductResponse
                    {
                        Id = Guid.NewGuid(),
                        Name = "Main Product",
                        IsActive = true
                    }
                };
            }
        }

        public class DeleteProductResponseSample : IExamplesProvider<ApiResponse>
        {
            public ApiResponse GetExamples()
            {
                return new ApiResponse
                {
                    Success = true,
                    Message = "Product deleted successfully",
                    Errors = new List<ValidationErrorDetail>()
                };
            }
        }
    }

    public class ProductErrorSamples
    {
        public class NotFoundErrorSample : IExamplesProvider<ApiResponse>
        {
            public ApiResponse GetExamples()
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Product not found",
                    Errors = new List<ValidationErrorDetail>
                    {
                        new ValidationErrorDetail { Error = "404", Detail = "Product with the specified ID does not exist" }
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
                        new ValidationErrorDetail { Error = "400", Detail = "Invalid Product data provided" }
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
