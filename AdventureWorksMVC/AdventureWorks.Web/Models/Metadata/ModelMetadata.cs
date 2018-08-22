using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AdventureWorks.Model.Models.Base;

namespace AdventureWorks.Web.Models.Metadata
{
    public class ModelMetadata<TModel>
        where TModel : BaseModel
    {
        #region Constructors

        public ModelMetadata()
        {
            PropertyList = new List<ModelPropertyMetadata>();

            Type type = typeof(TModel);

            foreach (var property in type.GetProperties())
            {
                ModelMetadata propertyMetadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, type, property.Name);

                PropertyList.Add(new ModelPropertyMetadata(propertyMetadata));
            }
        }

        #endregion

        #region Properties

        public List<ModelPropertyMetadata> PropertyList { get; protected set; }

        #endregion
    }
}
