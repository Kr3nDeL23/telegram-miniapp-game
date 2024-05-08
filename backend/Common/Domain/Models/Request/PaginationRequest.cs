namespace Presentation.Common.Domain.Models.Request;
public class PaginationRequest
{
    public int Page { get; set; } = 1;

    private int _size = 50;
    public int Size
    {
        get => _size;
        set => _size = Math.Min(value, _size);
    }
}
