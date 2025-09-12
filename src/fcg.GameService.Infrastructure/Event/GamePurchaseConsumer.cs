using Azure.Storage.Queues;
using fcg.GameService.Application.Interfaces;
using fcg.GameService.Domain.Event;
using fcg.GameService.Presentation.Event.Consume;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace fcg.GameService.Infrastructure.Event;

public class GamePurchaseConsumer : IConsumer<GamePurchaseConsumeEvent>
{
    private readonly QueueClient _client;
    private readonly IAppLogger<GamePurchaseConsumer> _logger;

    public GamePurchaseConsumer(IConfiguration config, IAppLogger<GamePurchaseConsumer> logger)
    {
        _client = new QueueClient(
            config["AzureStorage:ConsumerConnectionString"],
            config["AzureStorage:ConsumerQueueName"],
            new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });

        _client.CreateIfNotExists();
        _logger = logger;
    }

    public async Task<GamePurchaseConsumeEvent> ConsumeAsync(CancellationToken cancellationToken)
    {
        try
        {
            var message = await _client.ReceiveMessageAsync(cancellationToken: cancellationToken);
            _logger.LogInformation("Payment consumed successfully");

            if (message.Value == null)
            {
                _logger.LogWarning("No messages available in the queue.");
                return null!;
            }

            GamePurchaseConsumeEvent gamePurchaseEvent = JsonSerializer.Deserialize<GamePurchaseConsumeEvent>(message.Value.MessageText)!;
            await _client.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt, cancellationToken);
            return gamePurchaseEvent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to consume Payment}");
            throw;
        }
    }
}
