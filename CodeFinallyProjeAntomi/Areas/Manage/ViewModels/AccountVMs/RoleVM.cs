using System.ComponentModel.DataAnnotations;

namespace CodeFinallyProjeAntomi.Areas.Manage.ViewModels.AccountVMs
{
    public class RoleVM
    {
        [StringLength(255)]
        public IList<string> Roles { get; set; }
    }
}
