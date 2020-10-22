using BatchingLibrary.Core.Database;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BatchingLibrary.Main
{
    public interface IBatchLibrary
    {
        JobResult SendJob(JobModel model);
        IEnumerable<Guid> JobsProcessed(int limit = 5);
        void ConfigureLibrarySettings(int batchSize, int latencyInSeconds);
    }

    public class BatchLibrary : IBatchLibrary
    {
        private readonly IJobService _service;
        public BatchLibrary(IJobService service)
        {
            _service = service;
        }

        public void ConfigureLibrarySettings(int batchSize, int latencyInSeconds)
        {
            _service.ConfigureBatchSettings(batchSize, latencyInSeconds);
        }

        public JobResult SendJob(JobModel model)
        {
            try
            {
                if (model == null || model.Data == null)
                {
                    return new JobResult { Error = "Invalid payload.", Status = Core.Database.Entities.ProcessStatus.InvalidData };
                }

                return _service.CreateJob(model);
            }
            catch (Exception ex)
            {
                return new JobResult { Error = "Unhandled error while sending job.", Status = Core.Database.Entities.ProcessStatus.Failed };
                //log error
            }

        }

        public IEnumerable<Guid> JobsProcessed(int limit = 5)
        {
            return _service.GetProcessodJobs(limit);
        }

    }
}
