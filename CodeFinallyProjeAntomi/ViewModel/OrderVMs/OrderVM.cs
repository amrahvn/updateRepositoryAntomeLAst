using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.ViewModels.BasketVM;

namespace CodeFinallyProjeAntomi.ViewModel.OrderVMs
{
    public class OrderVM
    {
        public Order Order { get; set; }

        public IEnumerable<BasketVMs> BasketVM { get; set; }
    }
}
