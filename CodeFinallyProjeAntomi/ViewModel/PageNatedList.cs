using CodeFinallyProjeAntomi.Models;

namespace CodeFinallyProjeAntomi.ViewModels
{
    public class PageNatedList<T>:List<T> 
    {
        public PageNatedList(IQueryable<T> query, int currentPage, int totalPage, int pageItemCount, int elementCount)
        {
            CurrentPage = currentPage;
            TotalPage = totalPage;
            HasPrev = CurrentPage > 1;
            HasNext = CurrentPage < TotalPage;
            ElementCount = elementCount;
            Start = CurrentPage - (int)Math.Ceiling((decimal)(pageItemCount - 1) / 2);
            End = CurrentPage + (int)Math.Floor((decimal)(pageItemCount - 1) / 2);

            if (Start <= 0)
            {
                End = End - (Start - 1);
                Start = 1;
            }

            if (End > totalPage)
            {
                if (TotalPage > pageItemCount)
                {
                    Start = TotalPage - (pageItemCount - 1);
                }
                else
                {
                    Start = 1;
                }
                End = TotalPage;
            }

            this.AddRange(query);
        }

        public int CurrentPage { get; }
        public int ElementCount { get; }
        public int TotalPage { get; }
        public bool HasPrev { get; }
        public bool HasNext { get; }
        public int Start { get; }
        public int End { get; }

        public static PageNatedList<T> Create(IQueryable<T> query, int currentPage, int elemetCount, int pageItemCount)
        {
            int totalPage = (int)Math.Ceiling((decimal)query.Count() / elemetCount);
            query = query.Skip((currentPage - 1) * elemetCount).Take(elemetCount);

            return new PageNatedList<T>(query, currentPage, totalPage, pageItemCount, elemetCount);
        }
    }
}
