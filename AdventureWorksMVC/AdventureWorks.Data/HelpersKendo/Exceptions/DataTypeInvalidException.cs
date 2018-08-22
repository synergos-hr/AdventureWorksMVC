using System;

namespace AdventureWorks.Data.HelpersKendo.Exceptions
{
    internal class DataTypeInvalidException : Exception
    {
        public DataTypeInvalidException(string message)
            : base(message)
        {
        }
    }
}
