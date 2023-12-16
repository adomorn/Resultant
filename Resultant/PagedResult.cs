using System;
using System.Collections.Generic;

namespace Resultant
{
    public class PagedResult<T> : Result<List<T>>
    {
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        private PagedResult(List<T> value, int currentPage, int pageSize, int totalCount, bool isSuccess, IEnumerable<Error> errors)
            : base(value, isSuccess, errors)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        public static PagedResult<T> Create(List<T> value, int currentPage, int pageSize, int totalCount)
        {
            return new PagedResult<T>(value, currentPage, pageSize, totalCount, true, null);
        }

        public new static PagedResult<T> Fail(IEnumerable<Error> errors)
        {
            return new PagedResult<T>(null, 0, 0, 0, false, errors);
        }

        public new static PagedResult<T> Fail(string message, int code = 0)
        {
            return Fail(new List<Error> { new Error(message, code) });
        }
    }
}