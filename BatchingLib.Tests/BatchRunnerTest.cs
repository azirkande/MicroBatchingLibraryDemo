using BatchingLibrary.Core.Database;
using BatchingLibrary.BatchRunner;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using BatchingLibrary.Core.Database.Entities;
using Moq;

namespace BatchingLib.Tests
{
    public class BatchRunnerTest
    {

        private readonly IDbContext _dbContext;
        private Mock<IBatchProcessor> _processorMock;
        private readonly IBatchRunnerService _batchRunnerService;

        public BatchRunnerTest()
        {
            _dbContext = new DatabaseContext();
            _processorMock = new Mock<IBatchProcessor>();
            _processorMock.Setup(o => o.Process(It.IsAny<Batch>()));
            _batchRunnerService = new BatchRunnerService(_dbContext, _processorMock.Object);

        }

        [Fact]
        public void When_no_batch_available_to_process_and_no_previous_executions_happened()
        {
           
            var runner = new BatchingLibrary.BatchRunner.BatchRunner(_dbContext, _batchRunnerService);
            runner.SendBatchToProcessor();

            _processorMock.Verify(p => p.Process(It.IsAny<Batch>()), Times.Never());
            Assert.Null(_dbContext.ProcessorLastExecution.LastRun);
            Assert.Null(_dbContext.ProcessorLastExecution.BatchId);
        }

        [Fact]
        public void When_first_batch_available_to_process()
        {
            var batchId = Guid.NewGuid();
            var jobs = new List<Job>
            {
                new Job
                {
                    Id = Guid.NewGuid(),
                    Data = "ABC",
                    Status = ProcessStatus.Created
                }
            };

            _dbContext.Batches.Add(new Batch
            {
                CreatedOn = DateTime.Now,
                Id = batchId,
                Jobs = jobs,
                Status = ProcessStatus.Created
            });
            var runner = new BatchingLibrary.BatchRunner.BatchRunner(_dbContext, _batchRunnerService);
            runner.SendBatchToProcessor();

            _processorMock.Verify(p => p.Process(It.IsAny<Batch>()), Times.AtMostOnce);

            Assert.NotNull(_dbContext.ProcessorLastExecution.LastRun);
            Assert.Equal(batchId, _dbContext.ProcessorLastExecution.BatchId);
        }

        [Fact]
        public void Should_process_only_new_batches()
        {
            var batchId = Guid.NewGuid();
            var jobs = new List<Job>
            {
                new Job
                {
                    Id = Guid.NewGuid(),
                    Data = "ABC",
                    Status = ProcessStatus.Created
                }
            };

            _dbContext.Batches.Add(new Batch
            {
                CreatedOn = DateTime.Now,
                Id = batchId,
                Jobs = jobs,
                Status = ProcessStatus.Completed
            });

            _dbContext.Batches.Add(new Batch
            {
                CreatedOn = DateTime.Now,
                Id = batchId,
                Jobs = jobs,
                Status = ProcessStatus.Created
            });

           
            var runner = new BatchingLibrary.BatchRunner.BatchRunner(_dbContext, _batchRunnerService);
            runner.SendBatchToProcessor();

            _processorMock.Verify(p => p.Process(It.IsAny<Batch>()), Times.AtMostOnce);

            Assert.NotNull(_dbContext.ProcessorLastExecution.LastRun);
            Assert.Equal(batchId, _dbContext.ProcessorLastExecution.BatchId);
        }

        [Fact]
        public void Should_not_process_any_batch_if_processor_latency_not_met()
        {
            var lastBatch = Guid.NewGuid();

            _dbContext.BatchConfiguration.LatencyInSeconds = 10;
            _dbContext.ProcessorLastExecution.LastRun = DateTime.Now.AddSeconds(6);
            _dbContext.ProcessorLastExecution.BatchId = lastBatch;

            var batchId = Guid.NewGuid();
            var jobs = new List<Job>
            {
                new Job
                {
                    Id = Guid.NewGuid(),
                    Data = "ABC",
                    Status = ProcessStatus.Created
                }
            };

            _dbContext.Batches.Add(new Batch
            {
                CreatedOn = DateTime.Now,
                Id = batchId,
                Jobs = jobs,
                Status = ProcessStatus.Created
            });


            var runner = new BatchingLibrary.BatchRunner.BatchRunner(_dbContext, _batchRunnerService);
            runner.SendBatchToProcessor();

            _processorMock.Verify(p => p.Process(It.IsAny<Batch>()), Times.Never);

            Assert.Equal(lastBatch, _dbContext.ProcessorLastExecution.BatchId);
        }
    }
}
