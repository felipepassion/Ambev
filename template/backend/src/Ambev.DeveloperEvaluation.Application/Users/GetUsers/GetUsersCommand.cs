using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.GetUsers
{
    /// <summary>
    /// Command for retrieving a paginated list of users.
    /// </summary>
    public record GetUsersCommand(int PageNumber, int PageSize) : IRequest<List<GetUserResult>>;
}
