namespace AdventureWorks.Model.CustomFilters
{
    public class RequestProducts
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }

        public string Sort { get; set; }

        public int? SubcategoryID { get; set; }
    }
}
