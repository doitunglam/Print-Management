using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.Entities
{
    public class ResourcePropertyDetail : BaseEntity
    {
        public long PropertyId { get; set; }
        public virtual ResourceProperty? Property { get; set; }
        public string PropertyDetailName { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}
