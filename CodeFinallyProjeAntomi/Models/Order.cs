using CodeFinallyProjeAntomi.Enums;
using System.ComponentModel.DataAnnotations;

namespace CodeFinallyProjeAntomi.Models
{
    public class Order:BaseEntity
    {
        public int No { get; set; }

        public string? UserId { get; set; }

        public AppUser? User { get; set; }

        public Status Status { get; set; }

        public string? Comment { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Surnama { get; set; }

        [EmailAddress,StringLength(255)]
        public string Email { get; set; }

        [StringLength(255)]
        public string CompanyName { get; set; }

        [StringLength(2550)]
        public string County { get; set; }

        [StringLength(255)]
        public string Street { get; set; }

        [StringLength(255)]
        public string Street2 { get; set; }

        [StringLength(255)]
        public string Town { get; set; }

        [StringLength(255)]
        public string State { get; set; }

        [StringLength(255)]
        public string PostalCode { get; set; }

        [StringLength(255)]
        public string Phone { get; set; }

        [StringLength(2000)]
        public string OrderNotes { get; set; }

        public IEnumerable<OrderProduct>? OrderProducts { get; set; }
    }
}
