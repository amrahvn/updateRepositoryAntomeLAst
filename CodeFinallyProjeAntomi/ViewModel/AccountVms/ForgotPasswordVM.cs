using System.ComponentModel.DataAnnotations;

namespace CodeFinallyProjeAntomi.ViewModel.AccountVms
{
    public class ForgotPasswordVM
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}
