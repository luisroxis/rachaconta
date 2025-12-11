namespace RachaConta.Core.Entities
{
    public class PasswordResetToken : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }

        // Navigation property
        public User User { get; set; } = null!;

        public PasswordResetToken()
        {
        }

        public PasswordResetToken(Guid userId, string token, DateTime expiresAt)
        {
            UserId = userId;
            Token = token;
            ExpiresAt = expiresAt;
            IsUsed = false;
        }

        public bool IsValid()
        {
            return !IsUsed && DateTime.UtcNow < ExpiresAt;
        }
    }
}
