using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFinallyProjeAntomi.Models
{
    public class Product:BaseEntity
    {
        [StringLength(1000)]
        public string Title { get; set; }

        [Column(TypeName = "money")]
        public double Price { get; set; }

        [Column(TypeName = "money")]
        public double DisCountPrice { get; set; }

        [Column(TypeName = "money")]
        public double ExTag { get; set; }

        [Range(0,int.MaxValue)]
        public int Count { get; set; }

        [StringLength(1000)]
        public string SmallDescription { get; set; }

        [StringLength(5000)]
        public string Description { get; set; }

        [StringLength(4)]
        public string? Seria { get; set; }

        [Range(1, int.MaxValue)]
        public int? Number { get; set; }

        [StringLength(255)]
        public string? MainImage { get; set; }

        [StringLength(255)]
        public string? HoverImage { get; set; }

        public int? CategoryId { get; set; }

        public Category? Category { get; set; }

        public int? BrandId { get; set; }

        public Brand? Brand { get; set; }

        public bool NewArrival { get; set; }

        public bool BestSeller { get; set; }

        public bool Featured { get; set; }

        public List<ProductImage> productImages { get; set; }

        public List<ProductTag>? productTags { get; set; }

        public IEnumerable<Basket>? Baskets { get; set; }

        public IEnumerable<Wish>? Wishes { get; set; }

        public IEnumerable<OrderProduct>? OrderProducts { get; set; }

        [NotMapped]
        public IFormFile? MainFile { get; set; }

        [NotMapped]
        public IFormFile? HoverFile { get; set; }

        [NotMapped]
        public IEnumerable<IFormFile>? Files { get; set; }

        [NotMapped]
        public IEnumerable<int>? TagId1 { get; set; }

        public int Rating { get; set; }

        public ICollection<UserRating> UserRatings { get; set; }
    }
}
