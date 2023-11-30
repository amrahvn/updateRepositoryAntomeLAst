

using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.ViewModel.WishVMs;
using CodeFinallyProjeAntomi.ViewModels.BasketVM;

namespace CodeFinallyProjeAntomi.Interfaces
{
    public interface IlayoutService
    {
        Task<Dictionary<string, string>> GetSettingsAsync();
        Task<List<Category>> GetCategoriesAsync();
        Task<List<BasketVMs>> GetBasketsAsync();

        Task<List<WishVM>> GetWishListAsync();

    }
}
