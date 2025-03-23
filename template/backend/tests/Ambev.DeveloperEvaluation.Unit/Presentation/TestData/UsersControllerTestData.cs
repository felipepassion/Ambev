using Bogus;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.TestData
{
    /// <summary>
    /// Provides methods for generating test data (requests) for the UsersController.
    /// </summary>
    public static class UsersControllerTestData
    {
        private static readonly Faker<CreateUserRequest> _createUserFaker =
            new Faker<CreateUserRequest>()
                .RuleFor(r => r.Username, f => f.Internet.UserName())
                .RuleFor(r => r.Password, f => $"Test@{f.Random.Number(100, 999)}")
                .RuleFor(r => r.Email, f => f.Internet.Email())
                .RuleFor(r => r.Phone, f => $"+55{f.Random.Number(11, 99)}{f.Random.Number(100000000, 999999999)}")
                .RuleFor(r => r.Status, f => f.PickRandom(UserStatus.Active, UserStatus.Suspended))
                .RuleFor(r => r.Role, f => f.PickRandom(UserRole.Customer, UserRole.Manager, UserRole.Admin));

        /// <summary>
        /// Generates a valid CreateUserRequest with randomized data.
        /// </summary>
        public static CreateUserRequest GenerateValidCreateUserRequest()
        {
            return _createUserFaker.Generate();
        }

        /// <summary>
        /// Generates an invalid CreateUserRequest that fails validation.
        /// </summary>
        public static CreateUserRequest GenerateInvalidCreateUserRequest()
        {
            return new CreateUserRequest
            {
                Username = "",           // Required => invalid
                Password = "",           // Required => invalid
                Email = "not_an_email",  // Fails email format validator
                Phone = "",              // Fails phone validation
                Status = UserStatus.Unknown,
                Role = UserRole.None
            };
        }
    }
}
