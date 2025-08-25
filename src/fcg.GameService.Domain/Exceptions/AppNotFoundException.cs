namespace fcg.GameService.Domain.Exceptions;

public class AppNotFoundException : DomainException
{
    public AppNotFoundException(string message) : base(message) { }
    
    public static AppNotFoundException ForEntity(string entity, string id)
    {
        return new AppNotFoundException($"{entity} não encontrado com o ID {id}");
    }
}
