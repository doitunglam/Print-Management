namespace PM.Domain.Entities
{
    public class Permissions : BaseEntity
    {
        public long? UserId { get; set; }
        public virtual User? User { get; set; }
        public long? RoleId { get; set; }
        public virtual Role? Role { get; set; }
    }
}
