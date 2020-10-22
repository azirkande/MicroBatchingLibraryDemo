using BatchGenerator;
using BatchingLibrary.Core.Database;
using BatchingLibrary.Core.Database.Entities;
using System;
using System.Linq;
using Xunit;

namespace BatchingLib.Tests
{
    public class BatchGeneratorTest
    {
        private IDbContext _dbContext;
        private readonly IBatchService _batcService;

        public BatchGeneratorTest()
        {
            _dbContext = new DatabaseContext();
            _batcService = new BatchService(_dbContext);
        }


        [Fact]
        public void Should_generate_batch_when_enough_jobs_are_available()
        {
            _dbContext.BatchConfiguration.BatchSize = 2;

            Guid job1 = Guid.NewGuid();
            _dbContext.Jobs.Add(new Job
            {
                CreatedOn = DateTime.Now,
                Data = "First",
                Id = job1,
                Status = ProcessStatus.Created
            });

            Guid job2 = Guid.NewGuid();

            _dbContext.Jobs.Add(new Job
            {
                CreatedOn = DateTime.Now,
                Data = "Second",
                Id = job2,
                Status = ProcessStatus.Created
            });

            var generator = new BatchingLibrary.BatchGenerator.BatchGenerator(_batcService, _dbContext);
            generator.GenerateBatch();

            var batchCreated = _dbContext.Batches.First();
            Assert.Equal(ProcessStatus.Created, batchCreated.Status);
            Assert.Equal(2, batchCreated.Jobs.Count());
            Assert.NotNull(batchCreated.Jobs.First(j => j.Id == job1));
            Assert.NotNull(batchCreated.Jobs.First(j => j.Id == job2));
        }

        [Fact]
        public void Should_generate_batch_with_only_new_jobs()
        {
            Guid job3 = Guid.NewGuid();
            _dbContext.Jobs.Add(new Job
            {
                CreatedOn = DateTime.Now,
                Data = "First",
                Id = job3,
                Status = ProcessStatus.Completed
            });

            Guid job1 = Guid.NewGuid();
            _dbContext.Jobs.Add(new Job
            {
                CreatedOn = DateTime.Now,
                Data = "First",
                Id = job1,
                Status = ProcessStatus.Created
            });

            Guid job2 = Guid.NewGuid();

            _dbContext.Jobs.Add(new Job
            {
                CreatedOn = DateTime.Now,
                Data = "Second",
                Id = job2,
                Status = ProcessStatus.Created
            });

            var generator = new BatchingLibrary.BatchGenerator.BatchGenerator(_batcService, _dbContext);
            generator.GenerateBatch();

            var batchCreated = _dbContext.Batches.First();
            Assert.Equal(ProcessStatus.Created, batchCreated.Status);
            Assert.Equal(2, batchCreated.Jobs.Count());
            Assert.NotNull(batchCreated.Jobs.First(j => j.Id == job1));
            Assert.NotNull(batchCreated.Jobs.First(j => j.Id == job2));
        }

        [Fact]
        public void Should_not_generate_batch_when_enough_jobs_are_not_available()
        {
            _dbContext.BatchConfiguration.BatchSize = 3;

            _dbContext = new DatabaseContext();
            _dbContext.Jobs.Add(new Job
            {
                CreatedOn = DateTime.Now,
                Data = "First",
                Id = Guid.NewGuid(),
                Status = ProcessStatus.Created
            });

            var generator = new BatchingLibrary.BatchGenerator.BatchGenerator(_batcService, _dbContext);
            generator.GenerateBatch();

            Assert.Empty(_dbContext.Batches);
        }
    }
}
