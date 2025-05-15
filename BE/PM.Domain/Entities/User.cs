namespace PM.Domain.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Avatar { get; set; }
        public string Email { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string? PhoneNumber { get; set; }
        public long? TeamId { get; set; }
        public virtual Team? Team { get; set; }
        public virtual ICollection<Permissions> Permissions { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<ConfirmEmail> ConfirmEmails { get; set; }
    }
}
