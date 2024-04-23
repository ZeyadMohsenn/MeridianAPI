using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StoreManagement.API.Classes;

public class OpenApiLocalizationHeader : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Accept-Language",
            Required = false,
            In = ParameterLocation.Header,
            Description = "Localization Header",
            Schema = new OpenApiSchema
            {
                Type = "string"
            }

        });
    }
}

