using System.ComponentModel.DataAnnotations.Schema;

namespace EF.Examples.Domain
{
    [Table("ProductTags")]
    public class ProductTag : EntityBase
    {
        public required string Name { get; set; }

        public Guid ProductId { get; set; }

        public virtual Product? Product { get; set; }
    }
}
