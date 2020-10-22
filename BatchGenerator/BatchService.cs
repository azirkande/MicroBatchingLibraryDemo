using BatchingLibrary.Core.Database;
using BatchingLibrary.Core.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatchGenerator
{
    public interface IBatchService
    {
        IEnumerable<Job> GetNewJobs(int limit);
        void CreateBatch(IEnumerable<Job> jobs);
    }

    public class BatchService : IBatchService
    {
        private readonly IDbContext _dbContext;
        public BatchService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public IEnumerable<Job> GetNewJobs(int limit)
        {
            return _dbContext.Jobs.Where(job => job.Status == ProcessStatus.Created)
                .OrderBy(job => job.CreatedOn).Take(limit);
        }

        public void CreateBatch(IEnumerable<Job> jobs)
        {
            var batch = new Batch
            {
                Id = Guid.NewGuid(),
                Jobs = jobs,
                Status = ProcessStatus.Created,
                CreatedOn = DateTime.Now
            };

            _dbContext.Batches.Add(batch);
        }

    }
}
