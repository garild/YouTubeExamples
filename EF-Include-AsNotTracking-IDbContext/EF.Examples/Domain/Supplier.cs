using System.ComponentModel.DataAnnotations.Schema;

namespace EF.Examples.Domain
{
    [Table("Suppliers")]
    public class Supplier : EntityBase
    {
        public required string CompanyName { get; set; }

        public required string Email { get; set; }

        public required string Phone { get; set; }
    }
}
