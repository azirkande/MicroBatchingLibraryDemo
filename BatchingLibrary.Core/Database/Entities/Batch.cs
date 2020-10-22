using System;
using System.Collections.Generic;
using System.Text;

namespace BatchingLibrary.Core.Database.Entities
{
    public class Batch
    {
        public Guid Id { get; set; }
        public IEnumerable<Job> Jobs { get; set; }
        public ProcessStatus Status { get; set; }
        public DateTime CreatedOn {get;set;}
    }

}
