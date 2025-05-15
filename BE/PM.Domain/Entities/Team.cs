using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.Entities
{
    public class Team : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? NumberOfMember { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public long ManagerId { get; set; }
        public virtual User? Manager { get; set; }
        public ICollection<User> Users { get; set; }

        //public virtual ICollection<User>? Users { get; set; }

    }
}
