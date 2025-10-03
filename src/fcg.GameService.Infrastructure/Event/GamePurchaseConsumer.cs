using Azure.Storage.Queues;
using fcg.GameService.Application.Interfaces;
using fcg.GameService.Domain.Event;
using fcg.GameService.Presentation.Event.Consume;
using Microsoft.Extensions.Configuration;
using System.Text;
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
        Azure.Response<Azure.Storage.Queues.Models.QueueMessage>? message = null;
        
        try
        {
            message = await _client.ReceiveMessageAsync(cancellationToken: cancellationToken);
            _logger.LogInformation("Fila de pagamento consumida com sucesso");

            if (message.Value == null)
            {
                _logger.LogWarning("Não há mensagens na fila");
                return null!;
            }

            string decodedMessage = Encoding.UTF8.GetString(Convert.FromBase64String(message.Value.MessageText));
            GamePurchaseConsumeEvent gamePurchaseEvent = JsonSerializer.Deserialize<GamePurchaseConsumeEvent>(decodedMessage, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            })!;
            await _client.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt, cancellationToken);
            return gamePurchaseEvent;
        }
        catch (FormatException ex)
        {
            _logger.LogError(ex, "Mensagem Base64 inválida encontrada na fila. MessageId: {MessageId}", 
                message?.Value?.MessageId ?? "Unknown");
            
            // Tentar remover a mensagem corrompida da fila
            if (message?.Value != null)
            {
                try
                {
                    await _client.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt, cancellationToken);
                    _logger.LogInformation("Mensagem corrompida removida da fila com sucesso. MessageId: {MessageId}", message.Value.MessageId);
                }
                catch (Exception deleteEx)
                {
                    _logger.LogError(deleteEx, "Falha ao remover mensagem corrompida da fila. MessageId: {MessageId}", message.Value.MessageId);
                }
            }
            
            return null!;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erro ao deserializar JSON da mensagem. MessageId: {MessageId}", 
                message?.Value?.MessageId ?? "Unknown");
            
            // Remover mensagem com JSON inválido
            if (message?.Value != null)
            {
                try
                {
                    await _client.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt, cancellationToken);
                    _logger.LogInformation("Mensagem com JSON inválido removida da fila. MessageId: {MessageId}", message.Value.MessageId);
                }
                catch (Exception deleteEx)
                {
                    _logger.LogError(deleteEx, "Falha ao remover mensagem com JSON inválido. MessageId: {MessageId}", message.Value.MessageId);
                }
            }
            
            return null!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao consumir fila de pagamento");
            throw;
        }
    }
}
