using BatchingLibrary.Core.Database.Entities;
using System;


namespace BatchingLibrary.Main
{
    public class JobResult
    {
        public Guid? Id { get; set; }
        public ProcessStatus? Status { get; set; }
        public string Error { get; set; }
    }
}
