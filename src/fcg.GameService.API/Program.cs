using fcg.GameService.API.Middlewares;
using fcg.GameService.API.Workers;
using fcg.GameService.Application;
using fcg.GameService.Application.Interfaces;
using fcg.GameService.Application.Mappers;
using fcg.GameService.Infrastructure;
using fcg.GameService.Infrastructure.Adapters;
using fcg.GameService.Infrastructure.Configurations;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using System.Text.Json;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Inicializando aplicação");

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithSpan()
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] {TraceId} {SpanId} {Message:lj}{NewLine}{Exception}")
            .WriteTo.OpenTelemetry();
    });

    builder.Services.AddSingleton(typeof(IAppLogger<>), typeof(MicrosoftLogAdapter<>));

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

    // builder.Services.AddHostedService<GamePurchaseWorker>();

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

    WebApplication app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FCG Games v1");
        options.RoutePrefix = "swagger";
    });

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseMiddleware<BodyValidationMiddleware>();

    app.MapControllers();

    app.MapHealthChecks("/health");

    Log.Information("Aplicação inicializada com sucesso.");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro ao inicializar a aplicação.");
}
finally
{
    Log.CloseAndFlush();
}

