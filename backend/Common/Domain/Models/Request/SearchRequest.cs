namespace Presentation.Common.Domain.Models.Request;
public class SearchRequest
{
    private string _query;
    public string Query
    {
        get
        {
            return string.IsNullOrEmpty(_query) ? string.Empty : _query.ToLower();
        }
        set => _query = value;
    }

}
