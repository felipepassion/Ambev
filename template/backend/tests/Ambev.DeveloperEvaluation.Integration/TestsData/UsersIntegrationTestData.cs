using Bogus;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Unit.Presentation.TestData;

public static class UsersIntegrationTestData
{
    private static readonly Faker<CreateUserRequest> _userFaker =
        new Faker<CreateUserRequest>()
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.Password, f => $"Test@{f.Random.Number(100, 999)}")
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Phone, f => $"+55{f.Random.Number(11, 99)}{f.Random.Number(100000000, 999999999)}")
            .RuleFor(u => u.Status, f => f.PickRandom(UserStatus.Active, UserStatus.Suspended))
            .RuleFor(u => u.Role, f => f.PickRandom(UserRole.Customer, UserRole.Manager, UserRole.Admin));

    public static CreateUserRequest GenerateValidCreateUserRequest()
    {
        return _userFaker.Generate();
    }

    public static CreateUserRequest GenerateInvalidCreateUserRequest()
    {
        return new CreateUserRequest
        {
            Username = "",
            Password = "",
            Email = "invalid_email",
            Phone = "123",
            Status = UserStatus.Unknown,
            Role = UserRole.None
        };
    }
}
