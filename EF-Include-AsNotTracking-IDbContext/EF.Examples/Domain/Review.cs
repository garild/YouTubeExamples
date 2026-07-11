using System.ComponentModel.DataAnnotations.Schema;

namespace EF.Examples.Domain
{
    [Table("Reviews")]
    public class Review : EntityBase
    {
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public Guid ProductId { get; set; }

        public virtual Product? Product { get; set; }
    }
}
