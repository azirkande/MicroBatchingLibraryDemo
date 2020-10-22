using BatchingLibrary.Core.Database;
using BatchingLibrary.Main;
using BatchingLibrary.Core.Database.Entities;
using Xunit;

namespace BatchingLibrary.Test
{
    public class BatchLibraryTest
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository _repository;

        public BatchLibraryTest()
        {
            _dbContext = new DatabaseContext();
            _repository = new Repository(_dbContext);
        }

        [Fact]
        public void when_null_job_is_sent()
        {
            var library = new BatchLibrary(_repository);
            var result  = library.SendJob(null);
            Assert.Equal(ProcessStatus.InvalidData, result.Status);
            Assert.NotNull(result.Error);

        }

        [Fact]
        public void when_valid_job_is_sent()
        {
            var library = new BatchLibrary(_repository);
            var result = library.SendJob(new Core.Models.JobModel { Data = "This is a sample message"} );
            Assert.Equal(ProcessStatus.Created, result.Status);
            Assert.NotNull(result.Id);
        }

        [Fact]
        public void when_server_errors_out_after_job_is_sent()
        {
            var library = new BatchLibrary(_repository);
            var result = library.SendJob(new Core.Models.JobModel { Data = "abc" });
            Assert.Equal(ProcessStatus.Failed, result.Status);
            Assert.NotNull(result.Error);
        }
    }
}
