using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Presentation.Common.Domain.Enums;

namespace Presentation.Common.Domain.Entities;
public class BaseEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    public StateEnum State { get; set; }
    public DateTime DateCreation { get; set; }
    public DateTime DateModified { get; set; }
}
