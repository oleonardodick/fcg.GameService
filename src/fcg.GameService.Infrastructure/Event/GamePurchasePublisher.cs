using Azure.Storage.Queues;
using fcg.GameService.Application.Interfaces;
using fcg.GameService.Domain.Event;
using fcg.GameService.Presentation.Event.Publish;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace fcg.GameService.Infrastructure.Event;

public class GamePurchasePublisher : IPublisher<GamePurchasePublishEvent>
{
    private readonly QueueClient _client;
    private readonly IAppLogger<GamePurchasePublisher> _logger;

    public GamePurchasePublisher(IConfiguration config, IAppLogger<GamePurchasePublisher> logger)
    {
        _client = new QueueClient(
            config["AzureStorage:ProducerConnectionString"],
            config["AzureStorage:ProducerQueueName"],
            new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });

        _client.CreateIfNotExists();
        _logger = logger;
    }

    public async Task PublishAsync(GamePurchasePublishEvent message, CancellationToken cancellationToken)
    {
        try
        {
            string json = JsonSerializer.Serialize(message);

            _logger.LogInformation("Publishing payment to queue - PaymentId: {PaymentId}, Amount: {Amount}, Currency: {Currency}",
                message.PaymentId, message.Amount, message.Currency);
            await _client.SendMessageAsync(json, cancellationToken: cancellationToken);
            _logger.LogInformation("Payment published successfully - PaymentId: {PaymentId}", message.PaymentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish payment - PaymentId: {PaymentId}", message.PaymentId);
            throw;
        }
    }
}
