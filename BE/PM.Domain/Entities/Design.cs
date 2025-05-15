using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.Entities
{
    public class Design : BaseEntity
    {
        public long ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        public long DesignerId { get; set; }
        public virtual User? Designer { get; set; }
        public string FilePath { get; set; }
        public DateTime DesignTime { get; set; }
        public DesignStatus DesignStatus { get; set; }
        public long? ApproverId { get; set; }
    }

    public enum DesignStatus
    {
        Pending,
        Approved,
        Rejected
        // Add other statuses as needed
    }
}
