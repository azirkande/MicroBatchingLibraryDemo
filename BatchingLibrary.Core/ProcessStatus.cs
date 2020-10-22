using System;
using System.Collections.Generic;
using System.Text;

namespace BatchingLibrary.Core.Database.Entities
{
    public enum ProcessStatus
    {
        Scheduled,
        Created,
        Completed,
        Failed,
        InvalidData
    }
}
