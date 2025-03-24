using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;
using MediatR;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.ListUsers;

/// <summary>
/// Query for retrieving a paginated list of users.
/// </summary>
public class GetUsersQuery : IRequest<PaginatedList<GetUserResponse>>
{
    /// <summary>
    /// The page number to retrieve.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int PageSize { get; set; } = 10;
}
