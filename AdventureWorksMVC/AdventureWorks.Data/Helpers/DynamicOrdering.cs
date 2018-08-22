using System.Linq.Expressions;

namespace AdventureWorks.Data.Helpers
{
    internal class DynamicOrdering
    {
        public bool Ascending;
        public Expression Selector;
    }
}
