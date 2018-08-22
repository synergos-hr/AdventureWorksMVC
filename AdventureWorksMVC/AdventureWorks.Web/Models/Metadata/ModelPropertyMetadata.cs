using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AdventureWorks.Web.Models.Metadata
{
    public class ModelPropertyMetadata
    {
        #region Constructors

        public ModelPropertyMetadata(ModelMetadata metadata)
        {
            Field = metadata.PropertyName;

            Type = GetViewModelPropertyType(metadata.ModelType);

            Title = metadata.PropertyName;  // TODO: translate

            IsRequired = metadata.IsRequired;

            IsReadOnly = metadata.IsReadOnly;

            ProcessAdditionalValues(metadata.AdditionalValues);
        }

        #endregion

        #region Properties

        public string Field { get; set; }

        public string Type { get; set; }

        public string Title { get; set; }

        public bool IsKey { get; set; }

        public bool IsRequired { get; set; }

        public bool IsReadOnly { get; set; }

        public int MaxLength { get; set; }

        public int MinLength { get; set; }

        #endregion

        #region Methods

        private Type GetUnderlyingType(Type type)
        {
            Type coreType = type;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                coreType = Nullable.GetUnderlyingType(type);

            return coreType;
        }

        private string GetViewModelPropertyType(Type type)
        {
            Type coreType = GetUnderlyingType(type);

            if (coreType.Name.StartsWith("List"))
                return "list";

            switch (coreType.Name)
            {
                case "DateTime":
                    return "date";
                case "Boolean":
                case "bool":
                    return "boolean";
                case "Int16":
                case "Int32":
                case "Int64":
                case "Decimal":
                case "Double":
                case "Byte":
                    return "number";
                case "Byte[]":
                    return "upload";
                default:
                    return "string";
            }
        }

        private void ProcessAdditionalValues(Dictionary<string, object> additionalValues)
        {
            object additionalValue;

            if (additionalValues.TryGetValue("IsKey", out additionalValue))
                IsKey = (bool)additionalValue;

            if (additionalValues.TryGetValue("MaxLength", out additionalValue))
            {
                int? maxLength = (int?)additionalValue;

                if (maxLength.HasValue)
                    MaxLength = maxLength.Value;
            }

            if (additionalValues.TryGetValue("MinLength", out additionalValue))
            {
                int? minLength = (int?)additionalValue;

                if (minLength.HasValue)
                    MinLength = minLength.Value;
            }
        }

        #endregion
    }
}