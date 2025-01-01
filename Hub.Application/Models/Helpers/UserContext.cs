namespace Hub.Application.Models.Helpers
{
    public class UserContext
    {
        public string? AuthToken { get; set; }
        public long? CurrentUserId { get; set; }
    }

    public class AuthUserToken
    {
        public long UserId { get; set; }
        public long UserProfileId { get; set; }
    }

    public class ProfileControlAccess
    {
        public ProfileControlAccess(long userId, string cookieToken)
        {
            UserId = userId;
            Token = cookieToken;
            Creation = DateTime.Now;
        }

        public long UserId { get; set; }
        public string Token { get; set; }
        public DateTime Creation { get; set; }

        public override string ToString()
        {
            return $"portalUser_{UserId}";
        }
    }
}
