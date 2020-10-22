using BatchingLibrary.Core.Database;
using BatchingLibrary.Core.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BatchingLibrary.Main
{
    public interface IJobService
    {
        JobResult CreateJob(JobModel job);
        IEnumerable<Guid> GetProcessodJobs(int limit);
        void ConfigureBatchSettings(int batchSize, int latency);


    }
    public class JobService : IJobService
    {
        private readonly IDbContext _dbContext;
        public JobService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public void ConfigureBatchSettings(int batchSize, int latency)
        {
            _dbContext.BatchConfiguration.LatencyInSeconds = latency;
            _dbContext.BatchConfiguration.BatchSize = batchSize;
        }

        public JobResult CreateJob(JobModel incomingJob)
        {
            // This is just to test error condition

            if (incomingJob.Data is string && ((string)incomingJob.Data == "abc"))
                throw new Exception("Testing unhandled errors");

            var job = new Job
            {
                Id = Guid.NewGuid(),
                Status = ProcessStatus.Created,
                CreatedOn = DateTime.Now,
                Data = incomingJob.Data
            };
            _dbContext.Jobs.Add(job);
            return new JobResult { Id = job.Id, Status = job.Status };
        }

        public IEnumerable<Guid> GetProcessodJobs(int limit)
        {
            return _dbContext.Jobs.Where(job => job.Status == ProcessStatus.Completed).
                OrderBy(job => job.CreatedOn).Select(job => job.Id);
        }
    }
}
