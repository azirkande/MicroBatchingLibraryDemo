using BatchingLibrary.Core.Database.Entities;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace BatchingLibrary.Core.Database
{
    public interface IDbContext
    {
        List<Job> Jobs { get; set; }
        List<Batch> Batches { get; set; }
        BatchProcessorExecution ProcessorLastExecution { get; set; }
        BatchConfiguration BatchConfiguration { get; set; }
    }

    public class DatabaseContext : IDbContext
    {

        public DatabaseContext()
        {
            Jobs = new List<Job>();
            Batches = new List<Batch>();
            ProcessorLastExecution = new BatchProcessorExecution();
            BatchConfiguration = SetDefaultBatchConfiguration();

        }
        private BatchConfiguration SetDefaultBatchConfiguration()
        {
            return new BatchConfiguration()
            {
                BatchSize = 3,
                LatencyInSeconds = 3
            };
        }

        public List<Job> Jobs
        {
            get;
            set;
        }


        public List<Batch> Batches
        {
            get; set;
        }

        public BatchProcessorExecution ProcessorLastExecution
        {
            get; set;
        }

        public BatchConfiguration BatchConfiguration
        {
            get; set;
        }
        
    }
}
