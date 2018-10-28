using System;

namespace BlogHost.Models
{
    public class PageViewModel
    {
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }

        public PageViewModel(int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public bool NeedsBeginingElipsis
        {
            get
            {
                return (PageNumber > 3);
            }
        }

        public bool NeedsEndingElipsis
        {
            get
            {
                return (PageNumber < TotalPages - 2);
            }
        }

        public bool NeedsFirstPage
        {
            get
            {
                return (PageNumber > 2);
            }
        }

        public bool NeedsLastPage
        {
            get
            {
                return (PageNumber < TotalPages - 1);
            }
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageNumber > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageNumber < TotalPages);
            }
        }
    }
}