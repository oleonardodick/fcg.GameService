namespace fcg.GameService.Domain.Models;

public record ElasticLogRequest(
    int Page,
    int Size,
    string Index,
    string Field,
    string Value)
{
    public int Page { get; } = Page;
    public int Size { get; } = Size;
    public string Index { get; } = Index;
    public string Field { get; } = Field;
    public string Value { get; } = Value;
}
