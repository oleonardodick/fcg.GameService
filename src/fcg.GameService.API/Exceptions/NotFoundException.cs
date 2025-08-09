namespace fcg.GameService.API.Exceptions;

[Serializable]
public class NotFoundException : Exception
{
    public string Error { get; }
    public override string Message { get; }

    public NotFoundException(string error, string message) : base(message)
    {
        Error = error;
        Message = message;
    }
}
