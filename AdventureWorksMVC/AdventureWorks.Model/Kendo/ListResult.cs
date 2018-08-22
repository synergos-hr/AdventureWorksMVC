using System.Collections.Generic;

namespace AdventureWorks.Model.Kendo
{
    public class ListResult<T>
    {
        public ListResult()
        {
            Records = new List<T>();
            TotalCount = 0;
        }
        
        public string Status { get; set; }
        public string Message { get; set; }
        public IList<T> Records { get; set; }
        public int TotalCount { get; set; }
    }

    public class ListResult : GridResult<object>
    {
    }
}
