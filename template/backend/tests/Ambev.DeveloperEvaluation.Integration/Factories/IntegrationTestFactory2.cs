using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Integration.Routes;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.Integration.Factories;

public class IntegrationTestFactory2 : WebApplicationFactory<Program>
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
                options.UseInMemoryDatabase("IntegrationTestDb_Branches2");
            });

            services.AddControllers(options =>
            {
                options.Filters.Add<FakeUserFilter>();
            });

            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
                context.Database.EnsureCreated();

                context.Users.Add(new User
                {
                    Id = SalesIntegrationTests.User_ID,
                    Username = "john_doe_integration",
                    Email = "email@email.com",
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
