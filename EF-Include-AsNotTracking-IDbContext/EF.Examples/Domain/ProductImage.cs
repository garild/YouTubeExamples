using System.ComponentModel.DataAnnotations.Schema;

namespace EF.Examples.Domain
{
    [Table("ProductImages")]
    public class ProductImage : EntityBase
    {
        public required string Url { get; set; }

        public bool IsPrimary { get; set; }

        public Guid ProductId { get; set; }

        public virtual Product? Product { get; set; }
    }
}
