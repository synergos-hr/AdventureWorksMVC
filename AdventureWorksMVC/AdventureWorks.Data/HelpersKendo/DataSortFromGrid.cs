using AdventureWorks.Model.Kendo;
using System.Collections.Generic;
using System.Text;

namespace AdventureWorks.Data.HelpersKendo
{
    public static class DataSortFromGrid
    {
        public static string GetOrderClause(IEnumerable<GridRequestSort> requestSorts)
        {
            StringBuilder builder = new StringBuilder();

            foreach (GridRequestSort requestSort in requestSorts)
            {
                if (builder.Length > 0)
                    builder.Append(",");

                builder.AppendFormat("{0} {1}", requestSort.Field, requestSort.Dir);
            }

            return builder.ToString();
        }
    }
}
