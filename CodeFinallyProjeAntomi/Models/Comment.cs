using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFinallyProjeAntomi.Models
{
    public class Comment : BaseEntity
    {
        public string? AppUserID { get; set; }

        public AppUser? AppUser { get; set; }

        [StringLength(500)]
        public string? CommentText { get; set; }
    }
}
