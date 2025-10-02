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
            config["AzureStorage:ConnectionString"],
            config["AzureStorage:ConsumerQueueName"],
            new QueueClientOptions { MessageEncoding = QueueMessageEncoding.None });

        _client.CreateIfNotExists();
        _logger = logger;
    }

    public async Task<GamePurchaseConsumeEvent> ConsumeAsync(CancellationToken cancellationToken)
    {
        try
        {
            Azure.Response<Azure.Storage.Queues.Models.QueueMessage> message = await _client.ReceiveMessageAsync(cancellationToken: cancellationToken);
            _logger.LogInformation("Fila de pagamento consumida com sucesso");

            if (message.Value == null)
            {
                _logger.LogWarning("Não há mensagens na fila");
                return null!;
            }

            GamePurchaseConsumeEvent gamePurchaseEvent = JsonSerializer.Deserialize<GamePurchaseConsumeEvent>(message.Value.MessageText)!;
            await _client.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt, cancellationToken);
            return gamePurchaseEvent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao consumir fila de pagamento");
            throw;
        }
    }
}
