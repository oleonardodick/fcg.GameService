using fcg.GameService.API.Middlewares;
using fcg.GameService.Application;
using fcg.GameService.Application.Mappers;
using fcg.GameService.Infrastructure;
using fcg.GameService.Infrastructure.Configurations;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.OpenApi.Models;
using System.Text.Json;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FCG Games",
        Version = "v1",
        Description = "Microsserviço responsável pelo controle dos jogos da Fiap Cloud Games."
    });
});

builder.Services.AddProblemDetails();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddLoggingConfiguration(builder.Configuration);
builder.Services.AddApplication();
MappingConfig.RegisterMappings();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = true;
});

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FCG Games v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<BodyValidationMiddleware>();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
