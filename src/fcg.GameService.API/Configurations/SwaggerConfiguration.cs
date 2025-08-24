using fcg.GameService.Presentation.ProblemDefinitions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace fcg.GameService.API.Configurations;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class SwaggerResponseAttribute : Attribute
{
    public int StatusCode { get; }
    public Type? ResponseType { get; }
    public string Description { get; } = string.Empty;

    public SwaggerResponseAttribute(int statusCode, string description, Type? responseType)
    {
        StatusCode = statusCode;
        Description = description;
        ResponseType = responseType;
    }
}

public static class SwaggerConfiguration
{
    public static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FCG Games",
                Version = "v1",
                Description = "Microsserviço responsável pelo controle dos jogos da Fiap Cloud Games."
            });

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            options.MapType<NotFoundProblemDetails>(() => new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["type"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("Not Found") },
                    ["title"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("Resource not found") },
                    ["status"] = new OpenApiSchema { Type = "integer", Example = new OpenApiInteger(404) },
                    ["instance"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("METHOD /api/{endpoint}/{parameter}") },
                    ["details"] = new OpenApiSchema {Type = "string", Example = new OpenApiString("Mensagem de erro")},
                    ["traceId"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("00-abc123-def456-01") },
                    ["timestamp"] = new OpenApiSchema { Type = "string", Format = "date-time" }
                }
            });

            options.MapType<CustomValidationProblemDetails>(() => new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["type"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("https://httpstatuses.com/400") },
                    ["title"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("Um ou mais erros de validação aconteceram.") },
                    ["status"] = new OpenApiSchema { Type = "integer", Example = new OpenApiInteger(400) },
                    ["instance"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("METHOD /api/{endpoint}") },
                    ["details"] = new OpenApiSchema {Type = "string", Example = new OpenApiString("A requisição contém dados inválidos")},
                    ["errors"] = new OpenApiSchema
                    {
                        Type = "object",
                        AdditionalProperties = new OpenApiSchema
                        {
                            Type = "array",
                            Items = new OpenApiSchema { Type = "string" }
                        },
                        Example = new OpenApiObject
                        {
                            ["Propriedade"] = new OpenApiArray
                            {
                                new OpenApiString("Erro disparado na propriedade.")
                            },
                        }
                    },
                    ["traceId"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("00-abc123-def456-01") },
                    ["timestamp"] = new OpenApiSchema { Type = "string", Format = "date-time" },
                }
            });

            options.OperationFilter<SwaggerResponseOperationFilter>();
        });
    }

    public class SwaggerResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var responseAttributes = context.MethodInfo
                .GetCustomAttributes(typeof(SwaggerResponseAttribute), false)
                .Cast<SwaggerResponseAttribute>();

            foreach (var attr in responseAttributes)
            {
                var statusCode = attr.StatusCode.ToString();
                
                if (!operation.Responses.ContainsKey(statusCode))
                {
                    operation.Responses[statusCode] = new OpenApiResponse
                    {
                        Description = attr.Description
                    };
                }

                if (attr.ResponseType != null)
                {
                    var schema = context.SchemaGenerator.GenerateSchema(attr.ResponseType, context.SchemaRepository);
                    operation.Responses[statusCode].Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType { Schema = schema }
                    };
                }
            }
        }
    }

    public static void UseSwaggerConfiguration(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "FCG Games v1");
            options.RoutePrefix = string.Empty;
        });
    }
}
