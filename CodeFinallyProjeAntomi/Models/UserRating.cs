namespace CodeFinallyProjeAntomi.Models
{
    public class UserRating:BaseEntity
    {
        public string? UserId { get; set; }

        public AppUser? User { get; set; }

        public int? ProductId { get; set; }

        public Product? Product { get; set; }

        public int Rating { get; set; }

    }
}
