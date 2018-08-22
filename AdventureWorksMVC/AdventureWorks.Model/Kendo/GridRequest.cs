using System.Collections.Generic;

namespace AdventureWorks.Model.Kendo
{
    public class GridRequest
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Logic { get; set; }
        public IList<GridRequestSort> Sort { get; set; }
        public GridRequestFilterWrapper Filter { get; set; }
        public IDictionary<string, string> ExtraFilters { get; set; }
    }
}
