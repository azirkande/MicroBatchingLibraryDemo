using BatchingLibrary.Core.Database.Entities;

using System.Threading;

namespace BatchingLibrary.BatchRunner
{
    public interface IBatchProcessor
    {
        void Process(Batch batch);
    }
    public class BatchProcessor : IBatchProcessor
    {
        public void Process(Batch batch)
        {
            Thread.Sleep(3000);
        }
    }
}
