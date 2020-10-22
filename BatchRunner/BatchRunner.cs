using BatchingLibrary.Core.Database;

namespace BatchingLibrary.BatchRunner
{
    public interface IBatchRunner
    {
        void SendBatchToProcessor();
    }

    public class BatchRunner : IBatchRunner
    {
        private readonly IBatchRunnerService _batchRunnerService;

        private readonly IDbContext _dbContext;

        public BatchRunner(IDbContext dbContext, IBatchRunnerService batchRunnerService)
        {
            _dbContext = dbContext;
            _batchRunnerService = batchRunnerService;
        }

        public void SendBatchToProcessor()
        {
            _batchRunnerService.ProcessBatch();
        }
    }
}
