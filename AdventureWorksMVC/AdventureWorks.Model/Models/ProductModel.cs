using AdventureWorks.Model.Models.Base;

namespace AdventureWorks.Model.Models
{
    public class ProductModel : BaseModel
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string ProductNumber { get; set; }
        public string Color { get; set; }
        public decimal StandardCost { get; set; }
        public decimal ListPrice { get; set; }
        public string Size { get; set; }
        public string SizeUnitMeasureCode { get; set; }
        public decimal? Weight { get; set; }
        public string WeightUnitMeasureCode { get; set; }
        public int? ProductSubcategoryID { get; set; }

        public string SizeWithMeasure { get { return Size + " " + SizeUnitMeasureCode; } }
        public string WeightWithMeasure { get { return Weight + " " + WeightUnitMeasureCode; } }
        public string PriceWithCurrency { get { return string.Format("{0:c2}", ListPrice); } }
    }
}
