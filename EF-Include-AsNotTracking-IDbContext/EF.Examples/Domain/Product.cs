using System.ComponentModel.DataAnnotations.Schema;

namespace EF.Examples.Domain
{
    [Table("Products")]
    public class Product : EntityBase
    {
        public required string Name { get; set; }

        public required decimal Price { get; set; }

        public virtual ICollection<Category>? Category { get; set; }

        public virtual Supplier? Supplier { get; set; }

        public Guid? SupplierId { get; set; }

        public virtual ICollection<ProductImage> Images { get; set; } = [];

        public virtual ICollection<Review>? Reviews { get; set; } = [];

        public virtual ICollection<ProductTag>? Tags { get; set; } = [];

        public void AddImage(ProductImage image)
        {
            if (Images?.FirstOrDefault(i => i.Id == image.Id) is not null)
                throw new InvalidOperationException($"Image with Id {image.Id} already exists in the product.");
            Images?.Add(image);
        }

        public void AddCategory(string name)
        {
            Category ??= new List<Category>();
            Category.Add(new Category { Name = name });
        }
    }
}
