using Ambev.DeveloperEvaluation.Application.Branches.GetBranch;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Branches.GetBranch;

/// <summary>
/// Profile for mapping GetBranch feature requests to commands
/// </summary>
public class GetBranchProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetBranch feature
    /// </summary>
    public GetBranchProfile()
    {
        CreateMap<GetBranchRequest, GetBranchCommand>();
        CreateMap<GetBranchResult, GetBranchResponse>();
    }
}
