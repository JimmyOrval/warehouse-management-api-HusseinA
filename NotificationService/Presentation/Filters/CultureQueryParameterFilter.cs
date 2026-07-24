using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Presentation.Filters;

public class CultureQueryParameterFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<IOpenApiParameter>();
        
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "culture",
            In = ParameterLocation.Query,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Enum = new List<JsonNode>
                {
                    JsonValue.Create("en-US"),
                    JsonValue.Create("fr"),
                    JsonValue.Create("ar")
                },
                Description = "Response language"
            }
        });
    }
}