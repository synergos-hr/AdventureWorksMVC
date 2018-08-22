using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace AdventureWorks.Web.Helpers.Metadata
{
    public class CustomDataAnnotationsModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ICustomTypeDescriptor GetTypeDescriptor(Type type)
        {
            MetadataTypeAttribute metadata = type.GetCustomAttributes(typeof(MetadataTypeAttribute), true).OfType<MetadataTypeAttribute>().FirstOrDefault();

            if (metadata != null)
                return (new AssociatedMetadataTypeTypeDescriptionProvider(type, metadata.MetadataClassType)).GetTypeDescriptor(type);

            return base.GetTypeDescriptor(type);
        }

        public override ModelMetadata GetMetadataForType(Func<object> modelAccessor, Type modelType)
        {
            return base.GetMetadataForType(modelAccessor, modelType);
        }

        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            var modelMetadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);

            DisplayAttribute da = attributes.OfType<DisplayAttribute>().FirstOrDefault();

            if (da != null)
            {
                var autoGenerate = da.GetAutoGenerateFilter();

                if (autoGenerate.HasValue)
                    modelMetadata.AdditionalValues["AutoGenerateFilter"] = autoGenerate.Value;
                else
                    modelMetadata.AdditionalValues["AutoGenerateFilter"] = false;
            }

            modelMetadata.ShowForDisplay = true;
            modelMetadata.ShowForEdit = true;

            //var renderModeAttribute = attributes.OfType<RenderModeAttribute>();

            //if (renderModeAttribute.Any())
            //{
            //    var renderMode = renderModeAttribute.First().RenderMode;

            //    switch (renderMode)
            //    {
            //        case RenderMode.DisplayModeOnly:
            //            modelMetadata.ShowForDisplay = true;
            //            modelMetadata.ShowForEdit = false;
            //            break;
            //        case RenderMode.EditModeOnly:
            //            modelMetadata.ShowForDisplay = false;
            //            modelMetadata.ShowForEdit = true;
            //            break;
            //        case RenderMode.None:
            //            modelMetadata.ShowForDisplay = false;
            //            modelMetadata.ShowForEdit = false;
            //            break;
            //    }
            //}

            //var lookupMetaData = attributes.OfType<LookupAttribute>().FirstOrDefault();

            //if (lookupMetaData != null)
            //    modelMetadata.AdditionalValues.Add("LookupMetadata", lookupMetaData);

            //var lookupAdditionalMetaData = attributes.OfType<LookupAdditionalAttribute>().FirstOrDefault();

            //if (lookupAdditionalMetaData != null)
            //    modelMetadata.AdditionalValues.Add("LookupAdditionalMetadata", lookupAdditionalMetaData);

            //var multiLookupMetaData = attributes.OfType<MultiLookupAttribute>().FirstOrDefault();

            //if (multiLookupMetaData != null)
            //    modelMetadata.AdditionalValues.Add("MultiLookupMetadata", multiLookupMetaData);

            //var styleMetaData = attributes.OfType<StyleAttribute>().FirstOrDefault();

            //if (styleMetaData != null)
            //    modelMetadata.AdditionalValues.Add("StyleMetadata", styleMetaData);

            //string maxValue = null;
            //string minValue = null;

            ////decimal
            //var decimalMinValue = attributes.OfType<MinValueDecimalAttribute>().FirstOrDefault();

            //if (decimalMinValue != null && !String.IsNullOrWhiteSpace(decimalMinValue.MinValue))
            //    minValue = decimalMinValue.MinValue;

            //var decimalMaxValue = attributes.OfType<MaxValueDecimalAttribute>().FirstOrDefault();

            //if (decimalMaxValue != null && !String.IsNullOrWhiteSpace(decimalMaxValue.MaxValue))
            //    maxValue = decimalMaxValue.MaxValue;

            ////integer
            //var integerMinValue = attributes.OfType<MinValueIntegerAttribute>().FirstOrDefault();

            //if (integerMinValue != null && !String.IsNullOrWhiteSpace(integerMinValue.MinValue))
            //    minValue = integerMinValue.MinValue;

            //var integerMaxValue = attributes.OfType<MaxValueIntegerAttribute>().FirstOrDefault();

            //if (integerMaxValue != null && !String.IsNullOrWhiteSpace(integerMaxValue.MaxValue))
            //    maxValue = integerMaxValue.MaxValue;

            ////date
            //var dateMinValue = attributes.OfType<MinValueDateTimeAttribute>().FirstOrDefault();

            //if (dateMinValue != null && !String.IsNullOrWhiteSpace(dateMinValue.MinValue))
            //    minValue = dateMinValue.MinValue;

            //var dateMaxValue = attributes.OfType<MaxValueDateTimeAttribute>().FirstOrDefault();

            //if (dateMaxValue != null && !String.IsNullOrWhiteSpace(dateMaxValue.MaxValue))
            //    maxValue = dateMaxValue.MaxValue;

            //modelMetadata.AdditionalValues.Add("MinValue", minValue);

            //modelMetadata.AdditionalValues.Add("MaxValue", maxValue);

            var keyAttribute = attributes.OfType<KeyAttribute>().FirstOrDefault();

            if (keyAttribute != null)
                modelMetadata.AdditionalValues.Add("IsKey", true);

            var stringLengthAttribute = attributes.OfType<StringLengthAttribute>().FirstOrDefault();

            if (stringLengthAttribute != null)
            {
                modelMetadata.AdditionalValues.Add("MaxLength", stringLengthAttribute.MaximumLength);
                modelMetadata.AdditionalValues.Add("MinLength", stringLengthAttribute.MinimumLength);
            }

            var maxLengthAttribute = attributes.OfType<MaxLengthAttribute>().FirstOrDefault();

            if (maxLengthAttribute != null)
                modelMetadata.AdditionalValues.Add("MaxLength", maxLengthAttribute.Length);

            var minLengthAttribute = attributes.OfType<MinLengthAttribute>().FirstOrDefault();

            if (minLengthAttribute != null)
                modelMetadata.AdditionalValues.Add("MinLength", minLengthAttribute.Length);

            var regularExpressionAttribute = attributes.OfType<RegularExpressionAttribute>().FirstOrDefault();

            if (regularExpressionAttribute != null)
                modelMetadata.AdditionalValues.Add("Regex", regularExpressionAttribute.Pattern);

            return modelMetadata;
        }
    }
}