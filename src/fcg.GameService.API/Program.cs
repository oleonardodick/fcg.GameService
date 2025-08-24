using System.Text.Json;
using fcg.GameService.API.Middlewares;
using Microsoft.AspNetCore.Http.Json;
using fcg.GameService.Infrastructure;
using fcg.GameService.Application;
using fcg.GameService.API.Configurations;
using fcg.GameService.Application.Mappers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.AddInfrastructure(builder.Configuration);
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
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSwaggerConfiguration();

app.UseAuthorization();

app.UseMiddleware<BodyValidationMiddleware>();

app.MapControllers();

app.MapHealthChecks("/health");

app.UseExceptionHandler();

app.Run();
