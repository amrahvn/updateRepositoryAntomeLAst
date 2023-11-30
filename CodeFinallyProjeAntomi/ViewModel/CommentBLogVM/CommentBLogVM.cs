using CodeFinallyProjeAntomi.Models;
using CodeFinallyProjeAntomi.ViewModel.CommentVMs;

namespace CodeFinallyProjeAntomi.ViewModel.CommentBLogVM
{
    public class CommentBLogVM
    {
        public IEnumerable<Blog> Blogs { get; set; }

        public List<CommentVM> Comments { get; set; }
    }
}
