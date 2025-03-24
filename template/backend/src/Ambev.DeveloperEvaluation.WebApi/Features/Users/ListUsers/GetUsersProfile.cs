// GetUsersProfile.cs
using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.ListUsers;

/// <summary>
/// Profile for mapping User entities to GetUserResponse DTO for paginated user listings.
/// </summary>
public class GetUsersProfile : Profile
{
    public GetUsersProfile()
    {
        CreateMap<User, GetUserResponse>();
    }
}
