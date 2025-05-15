using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.Entities
{
    public class CustomerFeedback : BaseEntity
    {
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        public int? CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public string? FeedbackContent { get; set; }
        public string? ResponseByCompany { get; set; }
        public int? UserFeedbackId { get; set; }
        public virtual User? UserFeedback { get; set; }
        public DateTime? FeedbackTime { get; set; }
        public DateTime? ResponseTime { get; set; }
    }
}
