using System.ComponentModel.DataAnnotations;

namespace CodeFinallyProjeAntomi.ViewModel.AccountVms
{
    public class ResetPasswordVM
    {
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password),Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}

