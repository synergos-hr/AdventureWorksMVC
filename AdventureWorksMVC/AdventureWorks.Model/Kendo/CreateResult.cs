using System.Collections.Generic;

namespace AdventureWorks.Model.Kendo
{
    public class CreateResult<T> where T : class
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public IList<T> Records { get; set; }
    }
}
