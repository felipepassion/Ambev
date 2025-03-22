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
        CreateMap<Guid, Application.Branchs.GetBranch.GetBranchCommand>()
            .ConstructUsing(id => new Application.Branchs.GetBranch.GetBranchCommand(id));
    }
}
