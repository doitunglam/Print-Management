using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.Entities
{
    public class Resources : BaseEntity
    {
        public string ResourceName { get; set; }
        public string Image { get; set; }
        public ResourceType ResourceType { get; set; }
        public int AvailableQuantity { get; set; }
        public ResourceStatus ResourceStatus { get; set; }
    }

    public enum ResourceType
    {
        MayIn,
        Giay,
        Muc,
    }

    public enum ResourceStatus
    {
        SanSangSuDung,
        CanBaoTri,
        CanNhapThem
    }
}
