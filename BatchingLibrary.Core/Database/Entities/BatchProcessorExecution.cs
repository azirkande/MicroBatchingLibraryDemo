using System;


namespace BatchingLibrary.Core.Database.Entities
{
    public class BatchProcessorExecution
    {
        public Guid? BatchId { get; set; }
        public DateTime? LastRun { get; set; }
    }
}
