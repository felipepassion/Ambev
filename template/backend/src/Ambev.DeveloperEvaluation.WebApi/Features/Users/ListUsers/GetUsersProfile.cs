// GetUsersProfile.cs
using Ambev.DeveloperEvaluation.Application.Users.GetUsers;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.ListUsers;

/// <summary>
/// Profile for mapping User entities to GetUserResponse DTO for paginated user listings.
/// </summary>
public class GetUsersProfile : Profile
{
    public GetUsersProfile()
    {
        CreateMap<User, GetUserResponse>();
        CreateMap<GetUsersQuery, GetUsersCommand>();
    }
}
