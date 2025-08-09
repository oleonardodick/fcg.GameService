namespace fcg.GameService.API.Exceptions;

[Serializable]
public class DataValidationException : Exception
{
    public string Error { get; }
    public override string Message { get; }

    public DataValidationException(string error, string message) : base(message)
    {
        Error = error;
        Message = message;
    }
}
