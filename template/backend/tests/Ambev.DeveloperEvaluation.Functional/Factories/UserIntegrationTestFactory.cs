using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Functional.Extensions;
using Ambev.DeveloperEvaluation.Functional.Tests;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.Functional.Factories
{
    public class UserIntegrationTestFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DefaultContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Add in-memory database for integration tests
                services.AddDbContext<DefaultContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestDb-Users");
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
                    var _passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

                    context.Database.EnsureCreated();

                    // Seed usuário padrão
                    context.Users.Add(new User
                    {
                        Id = SalesIntegrationTests.User_ID,
                        Username = HttpClientExtensions.USERNAME,
                        Email = HttpClientExtensions.USEREMAIL,
                        Password = _passwordHasher.HashPassword(HttpClientExtensions.USERPASSWORD),
                        Status = Domain.Enums.UserStatus.Active
                    });

                    // Seed branches e produtos para outros testes
                    context.Branches.Add(new Branch
                    {
                        Id = SalesIntegrationTests.Branch_ID,
                        Name = "Main Branch"
                    });
                    context.Products.AddRange(
                        new Product { Id = SalesIntegrationTests.Product1_ID, Name = "Product 1", UnitPrice = 10 },
                        new Product { Id = SalesIntegrationTests.Product2_ID, Name = "Product 2", UnitPrice = 20 }
                    );

                    // Seed adicional para cenário de listagem de usuários
                    var additionalUsers = new List<User>
                    {
                        new User
                        {
                            Id = Guid.NewGuid(),
                            Username = "Alice",
                            Email = "alice@example.com",
                            Phone = "+551100000000",
                            Password = _passwordHasher.HashPassword("pwd"),
                            Status = Domain.Enums.UserStatus.Active,
                            Role = Domain.Enums.UserRole.Customer
                        },
                        new User
                        {
                            Id = Guid.NewGuid(),
                            Username = "Bob",
                            Email = "bob@example.com",
                            Phone = "+551100000001",
                            Password = _passwordHasher.HashPassword("pwd"),
                            Status = Domain.Enums.UserStatus.Active,
                            Role = Domain.Enums.UserRole.Customer
                        },
                        new User
                        {
                            Id = Guid.NewGuid(),
                            Username = "Charlie",
                            Email = "charlie@example.com",
                            Phone = "+551100000002",
                            Password = _passwordHasher.HashPassword("pwd"),
                            Status = Domain.Enums.UserStatus.Active,
                            Role = Domain.Enums.UserRole.Customer
                        },
                        new User
                        {
                            Id = Guid.NewGuid(),
                            Username = "David",
                            Email = "david@example.com",
                            Phone = "+551100000003",
                            Password = _passwordHasher.HashPassword("pwd"),
                            Status = Domain.Enums.UserStatus.Active,
                            Role = Domain.Enums.UserRole.Customer
                        },
                        new User
                        {
                            Id = Guid.NewGuid(),
                            Username = "Eve",
                            Email = "eve@example.com",
                            Phone = "+551100000004",
                            Password = _passwordHasher.HashPassword("pwd"),
                            Status = Domain.Enums.UserStatus.Active,
                            Role = Domain.Enums.UserRole.Customer
                        },
                        new User
                        {
                            Id = Guid.NewGuid(),
                            Username = "Frank",
                            Email = "frank@example.com",
                            Phone = "+551100000005",
                            Password = _passwordHasher.HashPassword("pwd"),
                            Status = Domain.Enums.UserStatus.Active,
                            Role = Domain.Enums.UserRole.Customer
                        }
                    };

                    context.Users.AddRange(additionalUsers);

                    context.SaveChanges();
                }
            });
        }
    }
}
