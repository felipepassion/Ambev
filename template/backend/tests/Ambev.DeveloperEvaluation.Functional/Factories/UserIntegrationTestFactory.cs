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

namespace Ambev.DeveloperEvaluation.Functional.Factories;

public class UserIntegrationTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DefaultContext>));
            if (descriptor != null)
                services.Remove(descriptor);

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

                context.Users.Add(new User
                {
                    Id = SalesIntegrationTests.User_ID,
                    Username = HttpClientExtensions.USERNAME,
                    Email = HttpClientExtensions.USEREMAIL,
                    Password = _passwordHasher.HashPassword(HttpClientExtensions.USERPASSWORD),
                    Status = Domain.Enums.UserStatus.Active
                });
                context.Branches.Add(new Branch
                {
                    Id = SalesIntegrationTests.Branch_ID,
                    Name = "Main Branch"
                });
                context.Products.AddRange(
                    new Product { Id = SalesIntegrationTests.Product1_ID, Name = "Product 1", UnitPrice = 10 },
                    new Product { Id = SalesIntegrationTests.Product2_ID, Name = "Product 2", UnitPrice = 20 }
                );
                context.SaveChanges();
            }
        });
    }
}
