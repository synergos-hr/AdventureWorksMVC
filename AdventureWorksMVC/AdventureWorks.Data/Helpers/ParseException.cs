using System;

namespace AdventureWorks.Data.Helpers
{
    public sealed class ParseException : Exception
    {
        private readonly int _position;

        public int Position
        {
            get { return _position; }
        }

        public ParseException(string message, int position)
            : base(message)
        {
            _position = position;
        }

        public override string ToString()
        {
            return string.Format("{0} (at index {1})", this.Message, this._position);
        }
    }
}
