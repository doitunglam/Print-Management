using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.Entities
{
    public class ResourceProperty : BaseEntity
    {
        public string ResourcePropertyName { get; set; }
        public long ResourceId { get; set; }
        public virtual Resources? Resource { get; set; }
        public int Quantity { get; set; }
    }
}
