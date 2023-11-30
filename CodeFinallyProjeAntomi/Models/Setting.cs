using System.ComponentModel.DataAnnotations;

namespace CodeFinallyProjeAntomi.Models
{
    public class Setting:BaseEntity
    {
        public int Id { get; set; }

        [StringLength(255)]
        public string? Key { get; set; }

        [StringLength(5000)]
        public string? Value { get; set; }

        public bool IsDeleted { get; set; }
    }
}
