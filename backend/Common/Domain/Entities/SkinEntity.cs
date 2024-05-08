namespace Presentation.Common.Domain.Entities;
public class SkinEntity : BaseEntity
{
    public string Title { get; set; }
    public long AvailableCoin { get; set; }

    public string Image { get; set; }
    public List<UserEntity> Users { get; set; } = new();
}
