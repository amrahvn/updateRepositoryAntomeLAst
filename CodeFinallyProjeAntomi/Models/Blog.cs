using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFinallyProjeAntomi.Models
{
    public class Blog:BaseEntity
    {
        [StringLength(5000)]
        public string? Title { get; set; }

        [StringLength(500)]
        public string? Image { get; set; }

        [NotMapped]
        public IFormFile? MainFile { get; set; }

        [StringLength(5000)]
        public string? news1 { get; set; }

        [StringLength(5000)]
        public string? news2 { get; set; }

        [StringLength(5000)]
        public string? news3 { get; set; }

        [StringLength(5000)]
        public string? news4 { get; set; }

    }
}
