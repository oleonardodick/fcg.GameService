using fcg.GameService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace fcg.GameService.Infrastructure.Adapters;

public class MicrosoftLogAdapter<T> : IAppLogger<T>
{
    private readonly ILogger<T> _logger;

    public MicrosoftLogAdapter(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogError(Exception exception, string message, params object[] args)
        => _logger.LogError(exception, message, args);

    public void LogInformation(string message, params object[] args)
        => _logger.LogInformation(message, args);

    public void LogWarning(string message, params object[] args)
        => _logger.LogWarning(message, args);
}
