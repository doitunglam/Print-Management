namespace PM.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public long UserId { get; set; }
        public virtual User? User { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}
