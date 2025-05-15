using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.Entities
{
    public class ResourceForPrintJob : BaseEntity
    {
        public long ResourcePropertyDetailId { get; set; }
        public virtual ResourcePropertyDetail? ResourcePropertyDetail { get; set; }
        public long PrintJobId { get; set; }
        public virtual PrintJobs? PrintJob { get; set; }
    }
}
