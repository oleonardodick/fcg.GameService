using fcg.Contracts;
using fcg.GameService.Application.Interfaces;
using fcg.GameService.Infrastructure.Configurations;
using MassTransit;
using Microsoft.Extensions.Configuration;

namespace fcg.GameService.Infrastructure.Event.Publishers;

public class GamePurchasePublisher : IMessagePublisher<GamePurchaseRequested>
{
    private readonly ISendEndpointProvider _endpointProvider;
    private readonly IAppLogger<GamePurchasePublisher> _logger;
    private readonly IConfiguration _configuration;

    public GamePurchasePublisher(ISendEndpointProvider endpointProvider, IAppLogger<GamePurchasePublisher> logger, IConfiguration configuration)
    {
        _endpointProvider = endpointProvider;
        _logger = logger;
        _configuration = configuration;  
    }
    public async Task PublishMessage(GamePurchaseRequested message, CancellationToken cancellationToken)
    {
        AWSSettings awsSettings = new();
        _configuration.GetSection(nameof(AWSSettings)).Bind(awsSettings);

        var endpoint = await _endpointProvider.GetSendEndpoint(new Uri($"queue:{awsSettings.SQS.GamePurchaseRequested}"));

        _logger.LogInformation("Publicando pagamento na fila - PaymentId: {PaymentId}, Amount: {Amount}, Currency: {Currency}",
                 message.PaymentId, message.Amount, message.Currency);

        await endpoint.Send(message, cancellationToken);

        _logger.LogInformation("Pagamento publicado com sucesso - PaymentId: {PaymentId}", message.PaymentId);
    }
}
