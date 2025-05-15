namespace PM.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string? RoleCode { get; set; }
        public string? RoleName { get; set; }

        public virtual ICollection<Permissions> Permissions { get; set; }
    }
}
