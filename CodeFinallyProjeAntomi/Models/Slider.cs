using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFinallyProjeAntomi.Models
{
    public class Slider:BaseEntity
    {
        [StringLength(255)]
        public string Image { get; set; }

        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(255)]
        public string MainTitle { get; set; }

        [StringLength(255)]
        public string SubTitle { get; set; }

        [StringLength(5005)]
        public string Description { get; set; }

        [StringLength(255)]
        public string Link { get; set; }

        [NotMapped]
        public IFormFile? File { get; set; }
    }
}
