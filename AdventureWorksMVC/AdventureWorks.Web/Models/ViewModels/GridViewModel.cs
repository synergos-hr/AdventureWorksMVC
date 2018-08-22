using AdventureWorks.Model.Models.Base;
using AdventureWorks.Web.Models.Metadata;

namespace AdventureWorks.Web.Models.ViewModels
{
    public class GridViewModel<TModel> : BaseViewModel
        where TModel : BaseModel
    {
        #region Constructors

        public GridViewModel()
        {
            Metadata = new ModelMetadata<TModel>();
        }

        #endregion

        #region Properties

        public ModelMetadata<TModel> Metadata { get; private set; }

        public int Test { get; set; }

        #endregion
    }
}
