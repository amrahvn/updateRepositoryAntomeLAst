using CodeFinallyProjeAntomi.Models;

namespace CodeFinallyProjeAntomi.ViewModels.HomeVms
{
    public class HomeVM
    {
        public IEnumerable<Slider> sliders { get; set; }

        public IEnumerable<Category> categories { get; set; }

        public IEnumerable<Product> products { get; set; }

        public IEnumerable<Product> NewArrival { get; set; }

        public IEnumerable<Product> BestSeller { get; set; }

        public IEnumerable<Product> Featured { get; set; }
    }
}
