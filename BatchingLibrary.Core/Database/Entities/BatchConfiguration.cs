using System;
using System.Collections.Generic;
using System.Text;

namespace BatchingLibrary.Core.Database.Entities
{
    public class BatchConfiguration
    {
        public int BatchSize { get; set; }
        public int LatencyInSeconds { get; set; }
    }
}
