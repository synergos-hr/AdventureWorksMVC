using System;

namespace AdventureWorks.Data.HelpersKendo.Exceptions
{
    internal class DataTypeNotSetException : Exception
    {
        public DataTypeNotSetException(string message)
            : base(message)
        {
        }
    }
}
