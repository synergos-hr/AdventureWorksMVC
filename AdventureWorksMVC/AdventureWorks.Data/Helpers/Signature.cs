using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventureWorks.Data.Helpers
{
    internal class Signature : IEquatable<Signature>
    {
        public int HashCode;
        public DynamicProperty[] Properties;

        public Signature(IEnumerable<DynamicProperty> properties)
        {
            Properties = properties.ToArray();
            
            HashCode = 0;

            foreach (DynamicProperty property in properties)
                HashCode ^= property.Name.GetHashCode() ^ property.Type.GetHashCode();
        }

        public bool Equals(Signature other)
        {
            if (Properties.Length != other.Properties.Length)
                return false;

            for (int i = 0; i < Properties.Length; i++)
            {
                if ((Properties[i].Name != other.Properties[i].Name) || (Properties[i].Type != other.Properties[i].Type))
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return ((obj is Signature) && Equals((Signature)obj));
        }

        public override int GetHashCode()
        {
            return HashCode;
        }
    }
}
