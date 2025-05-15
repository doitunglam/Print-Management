using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.Entities
{
    public class PrintJobs : BaseEntity
    {
        public long DesignId { get; set; }
        public virtual Design? Design { get; set; }
        public PrintJobStatus PrintJobStatus { get; set; }
    }

    public enum PrintJobStatus
    {
        InProgress,
        Completed
        // Add other statuses as needed
    }
}
