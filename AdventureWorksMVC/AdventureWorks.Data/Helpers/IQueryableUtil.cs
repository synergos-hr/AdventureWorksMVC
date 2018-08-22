using System.Linq;

namespace AdventureWorks.Data.Helpers
{
    internal static class IQueryableUtil<T>
    {
        public static int Count(IQueryable source)
        {
            return source.OfType<T>().AsQueryable<T>().Count<T>();
        }

        public static IQueryable Page(IQueryable source, int pageSize, int pageIndex)
        {
            return source.OfType<T>().AsQueryable<T>().Skip<T>((pageSize * (pageIndex - 1))).Take<T>(pageSize);
        }
    }
}
