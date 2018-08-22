using System.Collections.Generic;

namespace AdventureWorks.Model.Kendo
{
    public class GridRequestFilterWrapper
    {
        //public GridRequestFilterWrapper(string logic, IList<GridRequestFilter> filters)
        //{
        //    Logic = logic;
        //    Filters = filters;
        //}

        public string Logic { get; set; }

        public IList<GridRequestFilter> Filters { get; set; }

        //public string LogicToken
        //{
        //    get
        //    {
        //        switch (Logic)
        //        {
        //            case "and":
        //                return "&&";
        //            case "or":
        //                return "||";
        //            default:
        //                return null;
        //        }
        //    }
        //}
    }
}
