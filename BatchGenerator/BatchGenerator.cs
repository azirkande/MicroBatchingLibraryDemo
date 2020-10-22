using BatchGenerator;
using BatchingLibrary.Core.Database;
using System.Linq;

namespace BatchingLibrary.BatchGenerator
{
    public interface IBatchGenerator
    {
        void GenerateBatch();
    }

    public class BatchGenerator : IBatchGenerator
    {
        private readonly IBatchService _batchService;
        private readonly IDbContext _dbContext;

        public BatchGenerator(IBatchService batchService, IDbContext dbContext)
        {
            _batchService = batchService;
            _dbContext = dbContext;
        }
        public void GenerateBatch()
        {
            var batchSize = _dbContext.BatchConfiguration.BatchSize;
            var jobsToBatch = _batchService.GetNewJobs(batchSize);
            if (jobsToBatch != null && jobsToBatch.Count() == batchSize)
            {
                _batchService.CreateBatch(jobsToBatch);
            }

        }
    }
}
