namespace Identity.Data.Dtos.Response.Auth
{
    public class AuthResponse
    {
        public TokenResponse TokenResponse { get; set; }
        public UserProfile Profile { get; set; }
    }
}