using System.ComponentModel.DataAnnotations;

namespace CodeFinallyProjeAntomi.Areas.Manage.ViewModels.AccountVMs
{
    public class RoleVM
    {
        [StringLength(255)]
        public List<string> Roles { get; set; }
        public string Role { get; set; }
    }
}
