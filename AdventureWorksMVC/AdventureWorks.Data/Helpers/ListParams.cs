using System.Collections.Generic;
using AdventureWorks.Model.Kendo;

namespace AdventureWorks.Data.Helpers
{
    public class ListParams
    {
        public ListParams()
        {
        }

        public ListParams(string defaultSort)
        {
            DefaultSort = defaultSort;
        }

        public ListParams(GridRequest gridRequest, string defaultSort, string filter)
        {
            CustomFilters = gridRequest.ExtraFilters;
            Skip = gridRequest.Skip;
            Take = gridRequest.Take;

            DefaultSort = defaultSort;
            Filter = filter;
        }

        public string Filter { get; set; }
        public IDictionary<string, string> CustomFilters { get; set; }
        public string DefaultSort { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
