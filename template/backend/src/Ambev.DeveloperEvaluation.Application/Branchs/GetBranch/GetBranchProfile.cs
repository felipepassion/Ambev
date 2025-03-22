using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Branchs.GetBranch;

/// <summary>
/// Profile for mapping between Branch entity and GetBranchResponse
/// </summary>
public class GetBranchProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetBranch operation
    /// </summary>
    public GetBranchProfile()
    {
        CreateMap<Branch, GetBranchResult>();
    }
}
