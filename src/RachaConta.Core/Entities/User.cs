namespace RachaConta.Core.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public string Password { get; set; } = string.Empty;
        public bool TermOfUse { get; set; }
        public bool PrivacyPolicy { get; set; }

        public User()
        {
        }

        public User(string name, string userName, string email, string password, bool termOfUse, bool privacyPolicy, string? phone = null, string? avatar = null)
        {
            Name = name;
            UserName = userName;
            Email = email;
            Phone = phone;
            Avatar = avatar;
            Password = password;
            TermOfUse = termOfUse;
            PrivacyPolicy = privacyPolicy;
        }
    }
}
