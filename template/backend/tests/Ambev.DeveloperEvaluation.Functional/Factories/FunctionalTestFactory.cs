using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.FunctionalTests
{
    public class FunctionalTestFactory : WebApplicationFactory<Program>
    {
        public static Guid Product1_ID => new Guid("80B1DE5F-7DB1-470A-95CF-BD81F7A3FE05");
        public static Guid Product2_ID => new Guid("5F50BD28-AF1D-453C-98AF-73200DCBEE97");
        public static Guid Branch_ID => new Guid("D6EB9EE5-8544-4A4B-8863-0DB6707AD0C5");
        public static Guid User_ID => new Guid("8E9C5B87-49E4-4FE4-96DE-7889DAB989EC");

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
                    options.UseInMemoryDatabase("FunctionalTestDb");
                });

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
                    context.Database.EnsureCreated();

                    context.Users.Add(new User
                    {
                        Id = User_ID,
                        Username = "john_doe_integration",
                        Email = "email@email.com",
                    });
                    context.Branches.Add(new Branch
                    {
                        Id = Branch_ID,
                        Name = "Main Branch"
                    });
                    context.Products.AddRange(
                        new Product { Id = Product1_ID, Name = "Product 1", UnitPrice = 10 },
                        new Product { Id = Product2_ID, Name = "Product 2", UnitPrice = 20 }
                    );
                    context.SaveChanges();
                }
            });
        }
    }
}
