using System.Text.Json;
using fcg.GameService.API.DTOs.Requests;
using fcg.GameService.API.Handlers;
using fcg.GameService.API.Infrastructure.Configurations;
using fcg.GameService.API.Infrastructure.Services;
using fcg.GameService.API.Middlewares;
using fcg.GameService.API.Repositories.Implementations;
using fcg.GameService.API.Repositories.Interfaces;
using fcg.GameService.API.UseCases.Implementations;
using fcg.GameService.API.UseCases.Interfaces;
using fcg.GameService.API.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

var mongoSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

builder.Services.AddHealthChecks()
    .AddMongoDb(
        mongodbConnectionString: mongoSettings!.ConnectionString,
        name:"mongodb",
        timeout: TimeSpan.FromSeconds(5),
        tags: ["db", "mongo"]
    );

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection(nameof(MongoDbSettings))
);

builder.Services.AddSingleton<IMongoDbService, MongoDbService>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameUseCase, GameUseCase>();
builder.Services.AddScoped<IValidator<CreateGameDTO>, CreateGameDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateGameDTO>, UpdateGameDTOValidator>();
builder.Services.AddScoped<IGameLibraryRepository, GameLibraryRepository>();
builder.Services.AddScoped<IGameLibraryUseCase, GameLibraryUseCase>();

builder.Services.AddProblemDetails();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = true;
});

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

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
