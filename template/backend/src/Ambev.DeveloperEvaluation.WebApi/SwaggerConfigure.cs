using Ambev.DeveloperEvaluation.WebApi.Docs.Attributes;
using Ambev.DeveloperEvaluation.WebApi.Docs.Filters;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.WebApi;

/// <summary>
/// Configures Swagger for an API, setting up documentation, schemas, and operation filters. 
/// Integrates with the
/// service collection and application builder.
/// </summary>
public static class SwaggerConfigure
{
    /// <summary>
    /// Sets up Swagger for API documentation and configuration in the service collection.
    /// </summary>
    /// <param name="services">Used to register services required for Swagger functionality.</param>
    /// <param name="configuration">Provides settings and options for configuring the Swagger documentation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the services parameter is null.</exception>
    public static void AddSwaggerSetup(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddSwaggerGen(s =>
        {
            s.UseOneOfForPolymorphism();
            s.SelectDiscriminatorNameUsing((baseType) => "TypeName");
            s.SelectDiscriminatorValueUsing((subType) => subType.Name);
            s.DocumentFilter<OperationsOrderingFilter>();

            ConfigureDocumentation(configuration, s);
            ConfigureSchemas(s);

            s.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
            s.ExampleFilters();
            s.OperationFilter<SwaggerExcludeFilter>();
            s.OperationFilter<JsonIgnoreQueryOperationFilter>();
            s.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Developer Evaluation API",
                Description = @"API for managing sales, products, users, and branches. Implements business rules for discounting based on quantity and external identity patterns. 
<br/> <br/> <br/> 
<a target='_blank' href='https://localhost:44312/docs'>DOCUMENTATION LINK</a>
<br/><br/><br/> 
<b>Link Postman Test Collection: <b/> <a target='_blank' href='https://orange-meadow-3890.postman.co/workspace/My-Workspace~712ba6db-8893-4722-8a5f-f764e96900d7/collection/10408349-1d0fdc5b-e8ad-4084-82a3-5d0c01cf93f7?action=share&creator=10408349&active-environment=10408349-f1465d91-0ca4-44e7-bf43-fbb40990bc76'>Download</a>
<br/><br/><br/> 
<a target='_blank' href='https://localhost:44312/swagger/index.html'>CLICK HERE TO TEST BY SWAGGER API</a>",
                Contact = new OpenApiContact
                {
                    Name = "Felipe Paixão",
                    Email = string.Empty,
                    Url = new Uri("https://www.linkedin.com/in/felipepassion/"),
                },
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });
        });

        services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());

        services.AddSwaggerGenNewtonsoftSupport();
        services.AddMvcCore().AddApiExplorer();
    }

    /// <summary>
    /// Sets up Swagger for API documentation in the application pipeline.
    /// </summary>
    /// <param name="app">Configures the application builder to integrate Swagger functionality.</param>
    /// <param name="configuration">Provides access to application settings for configuring Swagger options.</param>
    /// <exception cref="ArgumentNullException">Thrown when the application builder is null.</exception>
    public static void UseSwaggerSetup(this IApplicationBuilder app, IConfiguration configuration)
    {
        if (app == null) throw new ArgumentNullException(nameof(app));

        app.UseSwagger(c =>
        {
            string basePath = "/";
            c.SerializeAsV2 = true;
            c.RouteTemplate = "{documentName}/swagger.json";
            c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            {
                swaggerDoc.Servers = new List<OpenApiServer>
                    { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{basePath}" } };
            });
        });
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/v1/swagger.json", "V1 Docs");
        });
    }

    /// <summary>
    /// Swagger schemsa and OperationFilters configuration
    /// </summary>
    /// <param name="options"></param>
    private static void ConfigureSchemas(SwaggerGenOptions options)
    {
        options.DescribeAllParametersInCamelCase();
    }

    /// <summary>
    /// Documentation XML path's and README configuration
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    private static void ConfigureDocumentation(IConfiguration configuration, SwaggerGenOptions options)
    {
        var path = PlatformServices.Default.Application.ApplicationBasePath;

        options.IncludeXmlComments(Path.Combine(path, $"Ambev.DeveloperEvaluation.WebApi.xml"), true);
        options.IncludeXmlComments(Path.Combine(path, $"Ambev.DeveloperEvaluation.Application.xml"), true);
        options.EnableAnnotations();
    }
}
