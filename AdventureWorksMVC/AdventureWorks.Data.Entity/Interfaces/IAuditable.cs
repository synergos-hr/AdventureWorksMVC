using System;

namespace AdventureWorks.Data.Entity.Interfaces
{
    public interface IAuditable
    {
        byte[] Timestamp { get; set; }

        int? LastModifiedBy { get; set; }

        DateTime LastModifiedDate { get; set; }
    }
}
