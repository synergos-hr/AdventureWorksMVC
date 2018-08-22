using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace AdventureWorks.Web.Helpers.Metadata
{
    public class CustomDataAnnotationsModelValidatorProvider : DataAnnotationsModelValidatorProvider
    {
        protected override ICustomTypeDescriptor GetTypeDescriptor(Type type)
        {
            MetadataTypeAttribute metadata = type.GetCustomAttributes(typeof(MetadataTypeAttribute), true).OfType<MetadataTypeAttribute>().FirstOrDefault();

            if (metadata != null)
                return (new AssociatedMetadataTypeTypeDescriptionProvider(type, metadata.MetadataClassType)).GetTypeDescriptor(type);

            return base.GetTypeDescriptor(type);
        }
    }
}