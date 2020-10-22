using BatchingLibrary.BatchRunner;
using BatchingLibrary.Core.Database;
using BatchingLibrary.Core.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatchingLibrary.BatchRunner
{
    public interface IBatchRunnerService
    {
        void ProcessBatch();
    }
    public class BatchRunnerService: IBatchRunnerService
    {
        private readonly IDbContext _dbContext;
        private readonly IBatchProcessor _batchProcessor;
        public BatchRunnerService(IDbContext dbContext, IBatchProcessor batchProcessor)
        {
            _dbContext = dbContext;
            _batchProcessor = batchProcessor;

        }

        private bool CanNextBatchBeProcessed(DateTime? lastRun, int latency)
        {
            if (!lastRun.HasValue)
                return true;
            var timeSinceLastBatchRun = DateTime.Now - lastRun.Value;
            return (timeSinceLastBatchRun.Seconds > latency);
        }

        public void ProcessBatch()
        {
            if (CanNextBatchBeProcessed(_dbContext.ProcessorLastExecution.LastRun, _dbContext.BatchConfiguration.LatencyInSeconds))
            {
                var nextBatch = GetNextNewBatchToProcess();

                if (nextBatch != null)
                {
                    var status = ProcessStatus.Completed;
                    try
                    {
                        _batchProcessor.Process(nextBatch);

                    }
                    catch (Exception ex)
                    {
                        status = ProcessStatus.Failed;
                        // log the exception 
                    }
                    UpdateBatchStatus(nextBatch.Id, status);
                    UpdateLastRunBatchProcessorExecution(nextBatch.Id, DateTime.Now);
                }

            }

        }
        private void UpdateJobStatus(Guid jobId, ProcessStatus status)
        {
            var jobTobeUpdated = _dbContext.Jobs.Where(job => job.Id == jobId).FirstOrDefault();
            if (jobTobeUpdated != null)
            {
                _dbContext.Jobs.Remove(jobTobeUpdated);
                jobTobeUpdated.Status = status;
                _dbContext.Jobs.Add(jobTobeUpdated);
            }
        }

        private void UpdateBatchStatus(Guid batchId, ProcessStatus status)
        {
            var batchTobeUpdated = _dbContext.Batches.Where(batch => batch.Id == batchId).FirstOrDefault();

            if (batchTobeUpdated != null)
            {
                _dbContext.Batches.Remove(batchTobeUpdated);
                batchTobeUpdated.Status = status;
                _dbContext.Batches.Add(batchTobeUpdated);

                batchTobeUpdated.Jobs.ToList().ForEach(job => UpdateJobStatus(job.Id, status));
            }

        }

        private Batch GetNextNewBatchToProcess()
        {
            return _dbContext.Batches.Where(batch => batch.Status == ProcessStatus.Created)
                   .OrderBy(batch => batch.CreatedOn).FirstOrDefault();
        }

        private void UpdateLastRunBatchProcessorExecution(Guid batchId, DateTime lastRun)
        {
            _dbContext.ProcessorLastExecution.BatchId = batchId;
            _dbContext.ProcessorLastExecution.LastRun = lastRun;
        }
    }
}
