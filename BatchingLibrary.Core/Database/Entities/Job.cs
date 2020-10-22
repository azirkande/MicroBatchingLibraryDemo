using System;

namespace BatchingLibrary.Core.Database.Entities
{
    public class Job
    {
        public Guid Id { get; set; }
        public object Data { get; set; }
        public ProcessStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
