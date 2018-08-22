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
            SortFilter sortFilter = new SortFilter();

            sortFilter.Sort = (request.Sort != null && request.Sort.Any()) ? DataSortFromGrid.GetOrderClause(request.Sort) : string.Format("{0} asc", defaultSortFieldName);

            sortFilter.Filter = DataFilterFromGrid.GetWhereClause<T>(request.Filter);

            return sortFilter;
        }
    }
}
