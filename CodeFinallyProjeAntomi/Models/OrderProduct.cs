using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CodeFinallyProjeAntomi.Models
{
    public class OrderProduct:BaseEntity
    {

        [StringLength(1000)]
        public string Title { get; set; }

        [Column(TypeName = "money")]
        public double Price { get; set; }

        [Column(TypeName = "money")]
        public double ExTag { get; set; }

        public int Count { get; set; }

        public int? ProductID { get; set; }

        public Product? Product { get; set; }

        public int? OrderId { get; set; }

        public Order? Order { get; set; }
    }
}
