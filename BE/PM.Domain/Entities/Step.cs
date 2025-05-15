using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.Entities
{
    public class Step : BaseEntity
    {
        public virtual Order? Order { get; set; }
        public string Name { get; set; }
        public DateTime? FulfilledAt { get; set; }
        public long Position { get; set; }
        public virtual string? Status { get; set; }
        public virtual string? Note { get; set; }
    }

    public class StepTemplate : BaseEntity
    {
        public string Name { get; set; } = "";
    }

    public class FlowTemplate : BaseEntity
    {
        public long ProductId { get; set; }
        public string Name { get; set; } = "";

        public ICollection<StepFlowTemplate> StepFlowTemplates { get; set; }
    }

    public class StepFlowTemplate : BaseEntity
    {
        public virtual long? FlowTemplateId { get; set; }

        public virtual long? StepTemplateId { get; set; }

        public int Position { get; set; }

        public string Name { get; set; }
    }
}
