using CodeFinallyProjeAntomi.Models;

namespace CodeFinallyProjeAntomi.ViewModel.AccountVms
{
    public class MyAccountVM
    {
        public ProfileAccountVM profileAccountVM { get; set; }

        public IEnumerable<Address> Addresses { get; set; }

        public IEnumerable<Order> Orders { get; set; }

        public Address Address { get; set; }
    }
}
