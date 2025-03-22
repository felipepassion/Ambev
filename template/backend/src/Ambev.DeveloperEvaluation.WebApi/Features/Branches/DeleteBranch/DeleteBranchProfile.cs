using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Branches.DeleteBranch;

/// <summary>
/// Profile for mapping DeleteBranch feature requests to commands
/// </summary>
public class DeleteBranchProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for DeleteBranch feature
    /// </summary>
    public DeleteBranchProfile()
    {
        CreateMap<Guid, Application.Branchs.DeleteBranch.DeleteBranchCommand>()
            .ConstructUsing(id => new Application.Branchs.DeleteBranch.DeleteBranchCommand(id));
    }
}
