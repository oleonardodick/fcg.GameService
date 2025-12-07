using Amazon.SimpleNotificationService;
using Amazon.SQS;
using fcg.Contracts;
using fcg.GameService.Infrastructure.Event.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace fcg.GameService.Infrastructure.Configurations;

public static class MassTransitSettings
{
    public static IServiceCollection AddMassTransitSettings(this IServiceCollection services, IConfiguration configuration)
    {
        // Carrega e valida a configuração real (env + appsettings)
        var awsSettings = AWSService.LoadAndValidate(configuration);

        // Registra o AWSSettings para ser injetado em serviços dependentes
        services.AddSingleton(awsSettings);

        services.AddMassTransit(x =>
        {
            x.AddConsumer<GamePurchaseConsumer>();

            x.AddConfigureEndpointsCallback((name, cfg) =>
            {
                cfg.UseMessageRetry(r =>
                {
                    r.Intervals(
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(60),
                        TimeSpan.FromSeconds(90),
                        TimeSpan.FromSeconds(120)
                    );
                });
            });

            x.UsingAmazonSqs((context, cfg) =>
            {
                cfg.Host(awsSettings.Region, h =>
                {
                    // CONFIGURAÇÃO PARA LOCALSTACK
                    if (!string.IsNullOrWhiteSpace(awsSettings.ServiceURL))
                    {
                        h.Config(new AmazonSQSConfig
                        {
                            ServiceURL = awsSettings.ServiceURL
                        });

                        h.Config(new AmazonSimpleNotificationServiceConfig
                        {
                            ServiceURL = awsSettings.ServiceURL
                        });
                    }

                    // OBS: Credenciais já estão definidas como Environment Variables
                    // por LoadAndValidate() – a AWS SDK usa automaticamente
                });

                // Seta os nomes das entidades SQS/SNS explicitamente
                cfg.Message<GamePurchaseCompleted>(m =>
                {
                    m.SetEntityName(awsSettings.SQS.GamePurchaseCompleted);
                });

                cfg.Message<GamePurchaseRequested>(m =>
                {
                    m.SetEntityName(awsSettings.SQS.GamePurchaseRequested);
                });

                // Endpoint que escuta a fila
                cfg.ReceiveEndpoint(awsSettings.SQS.GamePurchaseCompleted, e =>
                {
                    e.PrefetchCount = 5;
                    e.WaitTimeSeconds = 20;
                    e.ConfigureConsumeTopology = false;

                    e.ConfigureConsumer<GamePurchaseConsumer>(context);
                });
            });
        });

        return services;
    }
}