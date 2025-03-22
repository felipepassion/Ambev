using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Branchs.CreateBranch;

/// <summary>
/// Profile for mapping between Branch entity and CreateBranchResponse
/// </summary>
public class CreateBranchProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for CreateBranch operation
    /// </summary>
    public CreateBranchProfile()
    {
        CreateMap<CreateBranchCommand, Branch>();
        CreateMap<Branch, CreateBranchResult>();
    }
}
