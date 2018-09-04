using AdventureWorks.Model.Kendo;
using System.Linq;

namespace AdventureWorks.Data.HelpersKendo
{
    internal static class GridSortFilter
    {
        internal class SortFilter
        {
            public string Sort { get; set; }
            public string Filter { get; set; }
        }

        internal static SortFilter FromRequest<T>(GridRequest request, string defaultSortFieldName)
        {
            if (!(defaultSortFieldName.ToLower().EndsWith(" asc") || defaultSortFieldName.ToLower().EndsWith(" desc")))
                defaultSortFieldName += " asc";

            SortFilter sortFilter = new SortFilter
            {
                Sort = (request.Sort != null && request.Sort.Any())
                    ? DataSortFromGrid.GetOrderClause(request.Sort)
                    : $"{defaultSortFieldName}",
                Filter = DataFilterFromGrid.GetWhereClause<T>(request.Filter)
            };

            return sortFilter;
        }
    }
}
