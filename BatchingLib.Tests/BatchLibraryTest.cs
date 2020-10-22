using BatchingLibrary.Core.Database;
using BatchingLibrary.Main;
using BatchingLibrary.Core.Database.Entities;
using Xunit;

namespace BatchingLibrary.Test
{
    public class BatchLibraryTest
    {
        private readonly IDbContext _dbContext;
        private readonly IJobService _service;

        public BatchLibraryTest()
        {
            _dbContext = new DatabaseContext();
            _service = new JobService(_dbContext);
        }

        [Fact]
        public void When_null_job_is_sent()
        {
            var library = new BatchLibrary(_service);
            var result = library.SendJob(null);
            Assert.Equal(ProcessStatus.InvalidData, result.Status);
            Assert.NotNull(result.Error);

        }
        [Fact]
        public void When_invalid_job_is_sent()
        {
            var library = new BatchLibrary(_service);
            var result = library.SendJob(new JobModel());
            Assert.Equal(ProcessStatus.InvalidData, result.Status);
            Assert.NotNull(result.Error);

        }

        [Fact]
        public void When_valid_job_is_sent()
        {
            var library = new BatchLibrary(_service);
            var result = library.SendJob(new JobModel { Data = "This is a sample message" });
            Assert.Equal(ProcessStatus.Created, result.Status);
            Assert.NotNull(result.Id);
        }

        [Fact]
        public void When_server_errors_out_after_job_is_sent()
        {
            var library = new BatchLibrary(_service);
            var result = library.SendJob(new JobModel { Data = "abc" });
            Assert.Equal(ProcessStatus.Failed, result.Status);
            Assert.NotNull(result.Error);
        }
    }
}
