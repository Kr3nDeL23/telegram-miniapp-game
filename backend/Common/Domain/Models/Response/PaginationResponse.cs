using Presentation.Common.Domain.Models.Request;

namespace Presentation.Common.Domain.Models.Response;
public class PaginationResponse<T> : Response
{
    public int CountPage { get; set; }
    public int CountTotal { get; set; }

    public PaginationResponse(List<T> values, PaginationRequest request) : base(true)
    {
        Result = values;
        CountTotal = values.Count();
        CountPage = (int)Math.Ceiling(values.Count() / (double)request.Size);
    }
}
