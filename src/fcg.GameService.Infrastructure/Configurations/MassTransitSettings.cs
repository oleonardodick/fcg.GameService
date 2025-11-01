using Amazon.SimpleNotificationService;
using Amazon.SQS;
using fcg.Contracts;
using fcg.GameService.Infrastructure.Event.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace fcg.GameService.Infrastructure.Configurations;

public static class MassTransitSettings {
    public static IServiceCollection AddMassTransitSettings(this IServiceCollection services, IConfiguration configuration)
    {
        AWSService.SetAWSSettingsFromConfiguration(configuration);

        AWSSettings awsSettings = new();
        configuration.GetSection(nameof(AWSSettings)).Bind(awsSettings);

        if (string.IsNullOrEmpty(awsSettings.SQS.GamePurchaseRequested))
            throw new ConfigurationException("Parâmetro GamePurchaseRequested não configurado");

        if (string.IsNullOrEmpty(awsSettings.SQS.GamePurchaseCompleted))
            throw new ConfigurationException("Parâmetro GamePurchaseCompleted não configurado");

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
                    if (!string.IsNullOrEmpty(awsSettings.ServiceURL))
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
                });

                cfg.Message<GamePurchaseCompleted>(m =>
                {
                    m.SetEntityName(awsSettings.SQS.GamePurchaseCompleted);
                });

                cfg.Message<GamePurchaseRequested>(m =>
                {
                    m.SetEntityName(awsSettings.SQS.GamePurchaseRequested);
                });

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