using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ambev.DeveloperEvaluation.WebApi.Docs.Filters;

public class JsonIgnoreQueryOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        context.ApiDescription.ParameterDescriptions
            .Where(d => true).ToList()
            .ForEach(param =>
            {
                var toIgnore =
                    ((DefaultModelMetadata)param.ModelMetadata)
                    .Attributes.PropertyAttributes
                    ?.Any(x => x is Newtonsoft.Json.JsonIgnoreAttribute);

                var toRemove = operation.Parameters
                    .SingleOrDefault(p => p.Name.Equals(param.Name, StringComparison.InvariantCultureIgnoreCase));

                if (toIgnore ?? false)
                    operation.Parameters.Remove(toRemove);
            });
    }
}
