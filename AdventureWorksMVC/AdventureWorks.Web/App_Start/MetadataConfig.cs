using AdventureWorks.Web.Helpers.Metadata;
using System.Web.Mvc;

namespace AdventureWorks.Web
{
    public static class MetadataConfig
    {
        public static void SetMetadataProvider()
        {
            //DefaultModelBinder.ResourceClassKey = "ErrorMessages";

            //ClientDataTypeModelValidatorProvider.ResourceClassKey = "ErrorMessages";

            //DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute), typeof(ValidationRequiredAttributeAdapter));
            //DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(System.ComponentModel.DataAnnotations.RangeAttribute), typeof(ValidationRangeAttributeAdapter));
            //DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(System.ComponentModel.DataAnnotations.StringLengthAttribute), typeof(ValidationStringLengthAttributeAdapter));

            ModelMetadataProviders.Current = new CustomDataAnnotationsModelMetadataProvider();

            ModelValidatorProviders.Providers.Clear();
            ModelValidatorProviders.Providers.Add(new CustomDataAnnotationsModelValidatorProvider());
        }
    }
}