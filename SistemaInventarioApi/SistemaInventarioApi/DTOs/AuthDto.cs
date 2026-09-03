namespace SistemaInventarioApi.DTOs
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class TokenResponseDto
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
    }
}
