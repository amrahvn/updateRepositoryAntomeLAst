using System.ComponentModel.DataAnnotations;

namespace CodeFinallyProjeAntomi.Models
{
    public class Brand:BaseEntity
    {
        [StringLength(255)]
        public string Name { get; set; }

        public IEnumerable<Product>? products { get; set; }
    }
}
