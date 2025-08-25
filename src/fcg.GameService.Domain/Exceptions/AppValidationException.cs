using fcg.GameService.Domain.Models;

namespace fcg.GameService.Domain.Exceptions;

public class AppValidationException : DomainException
{
    public List<ErrorDetails> Errors { get; } = [];

    public AppValidationException(List<ErrorDetails> errors)
        : base("Um ou mais erros foram encontrados.")
    {
         Errors = errors ?? [];
    }

    // public AppValidationException(string field, string error)
    //     : this(new Dictionary<string, List<string>> { { field, new List<string> { error } } })
    // {
        
    // }
}
