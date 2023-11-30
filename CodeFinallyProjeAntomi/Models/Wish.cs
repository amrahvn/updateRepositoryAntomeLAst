namespace CodeFinallyProjeAntomi.Models
{
    public class Wish:BaseEntity
    {
        public int Count { get; set; }

        public int? ProductID { get; set; }

        public Product? Product { get; set; }

        public string? AppUserID { get; set; }

        public AppUser? AppUser { get; set; }
    }
}
