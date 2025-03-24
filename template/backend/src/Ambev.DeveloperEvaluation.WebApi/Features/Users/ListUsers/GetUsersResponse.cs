using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.ListUsers
{
    /// <summary>
    /// API response model for the GetUsers operation providing a paginated list of users.
    /// </summary>
    public class GetUsersResponse : PaginatedResponse<GetUserResponse>
    {
    }
}
