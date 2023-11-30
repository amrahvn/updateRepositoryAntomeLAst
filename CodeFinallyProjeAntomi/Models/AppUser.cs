using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFinallyProjeAntomi.Models
{
    public class AppUser:IdentityUser
    {
        [NotMapped]
        public IList<string> Roles { get; set; }   

        public bool IsActive { get; set; }

        [StringLength(255)]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Surname { get; set; }

        public IEnumerable<Address>? Address { get; set; }

        public IEnumerable<Basket>? Baskets { get; set; }

        public IEnumerable<Wish>? Wishes { get; set; }

        public IEnumerable<Order>? Order { get; set; }

        public IEnumerable<UserRating>? UserRating { get; set; }
    }
}
