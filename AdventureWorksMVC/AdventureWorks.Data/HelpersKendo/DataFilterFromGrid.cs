using AdventureWorks.Data.HelpersKendo.Exceptions;
using AdventureWorks.Model.Kendo;
using System;
using System.Reflection;

namespace AdventureWorks.Data.HelpersKendo
{
    public static class DataFilterFromGrid
    {
        public static bool CaseSensitive = false;

        public static string GetWhereClause<T>(GridRequestFilterWrapper filterWrapper)
        {
            if (filterWrapper == null || filterWrapper.Filters == null)
                return "";

            string clause = "";

            foreach (GridRequestFilter filter in filterWrapper.Filters)
            {
                if (filter.Value == null)
                    continue;

                clause += (clause.Length > 0) ? " " + filterWrapper.Logic + " " : "";

                clause += GetFieldWhereClause<T>(filter);
            }

            if (!string.IsNullOrEmpty(clause))
                clause = "(" + clause + ")";

            return clause;
        }

        private static string GetFieldWhereClause<T>(GridRequestFilter filter)
        {
            var property = typeof(T).GetProperty(filter.Field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (property == null)
                throw new DataTypeInvalidException(string.Format("Column type ({0}) must be set in order to perform search operations.", filter.Field));

            string dataType =
                (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    ? property.PropertyType.GetGenericArguments()[0].Name.ToLower()
                    : property.PropertyType.Name.ToLower();

            //string filterExpressionCompare;

            //switch (dataType)
            //{
            //    case "string":
            //        filterExpressionCompare = "{0} {1} \"{2}\"";
            //        break;
            //    case "int":
            //    case "float":
            //    case "currency":
            //    case "percent":
            //    case "money":
            //        filterExpressionCompare = "{0} {1} {2}";
            //        break;
            //    case "datetime":
            //        //DateTime date = DateTime.Parse(requestSearch.Value.ToString());
            //        //filterExpressionCompare = "{0} {1} DateTime" + string.Format("({0},{1},{2})", date.Year, date.Month, date.Day);

            //        filterExpressionCompare = "{0} {1} {2}";
            //        break;
            //    default:
            //        throw new DataTypeInvalidException(string.Format("Column type ({0}) must be set in order to perform search operations.", dataType));
            //}

            //return (string.Format("({0} != null AND {1})", filter.Field, GetLinqExpression(filter, filterExpressionCompare)));

            string field = filter.Field;
            string value = filter.Value;

            if (dataType == "string")
            {
                if (!CaseSensitive)
                {
                    field = field.ToLower();
                    value = value.ToLower();
                }

                value = @"""" + value + @"""";
            }

            if (dataType == "datetime")
            {
                var date = DateTime.Parse(value);

                value = string.Format("DateTime({0}, {1}, {2})", date.Year, date.Month, date.Day);
            }

            string expression;

            switch (filter.Operator)
            {
                case "eq":
                    expression = string.Format("{0} == {1}", field, value);
                    break;

                case "neq":
                    expression = string.Format("{0} != {1}", field, value);
                    break;

                case "contains":
                    expression = string.Format("{0}.Contains({1})", field, value);
                    break;

                case "startswith":
                    expression = string.Format("{0}.StartsWith({1})", field, value);
                    break;

                case "endswith":
                    expression = string.Format("{0}.EndsWith({1})", field, value);
                    break;

                case "gte":
                    expression = string.Format("{0} >= {1}", field, value);
                    break;

                case "gt":
                    expression = string.Format("{0} > {1}", field, value);
                    break;

                case "lte":
                    expression = string.Format("{0} <= {1}", field, value);
                    break;

                case "lt":
                    expression = string.Format("{0} < {1}", field, value);
                    break;

                case "notnull":
                    expression = string.Format("{0} != null", field);
                    break;

                case "null":
                    expression = string.Format("{0} == null", field);
                    break;

                default:
                    return "";
            }

            return string.Format(expression, field, value);
        }
    }
}
