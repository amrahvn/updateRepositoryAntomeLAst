using CodeFinallyProjeAntomi.Enums;
using System.ComponentModel.DataAnnotations;

namespace CodeFinallyProjeAntomi.Areas.Manage.ViewModels.OrderVms
{
    public class OrderVm
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        [StringLength(255)]
        public string? Comment { get; set; }

        public Status Status { get; set; }
    }
}
