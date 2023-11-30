using System.ComponentModel.DataAnnotations;

namespace CodeFinallyProjeAntomi.ViewModel.AccountVms
{
    public class RegisterVM
    {
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Surname { get; set; }

        [StringLength(255)]
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfiremPassword { get; set; }
    }
}
