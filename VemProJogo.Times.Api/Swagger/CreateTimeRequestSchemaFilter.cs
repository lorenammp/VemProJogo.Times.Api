using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using VemProJogo.Times.Application.DTOs.Times;

namespace VemProJogo.Times.Api.Swagger;

public sealed class CreateTimeRequestSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(CreateTimeRequest))
        {
            return;
        }

        schema.Example = new OpenApiObject
        {
            ["championshipId"] = new OpenApiString("67f07a3c9a9d8f4b21c0a111"),
            ["name"] = new OpenApiString("Tigres da Serra"),
            ["acronym"] = new OpenApiString("TDS"),
            ["responsibleName"] = new OpenApiString("Mariana Costa"),
            ["responsibleContact"] = new OpenApiString("+55 (31) 98888-1122"),
            ["crestUrl"] = new OpenApiString("https://fake-cdn.vemprojogo.local/escudos/tigres-da-serra.png")
        };
    }
}
