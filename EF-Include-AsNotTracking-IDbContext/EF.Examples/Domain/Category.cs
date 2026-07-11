using System.ComponentModel.DataAnnotations.Schema;

namespace EF.Examples.Domain;

[Table("Categories")]
public class Category : EntityBase
{
    public required string Name { get; set; }

    public virtual ICollection<Product>? Products { get; set; } = [];
}