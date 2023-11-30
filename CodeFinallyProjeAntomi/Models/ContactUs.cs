using System.ComponentModel.DataAnnotations;

namespace CodeFinallyProjeAntomi.Models
{
    public class ContactUs:BaseEntity
    {
        [StringLength(255)]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [StringLength(1000)]
        public string Subject { get; set; }

        [StringLength(5000)]
       public string Message { get; set; }
    }
}
