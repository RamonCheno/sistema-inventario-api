using System.ComponentModel.DataAnnotations;

namespace SistemaInventarioApi.DTOs
{
    public class LoginDto
    {
        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "El correo es obligatorio.")]
        [SistemaInventarioApi.Validation.EmailFormat(
            ErrorMessage = "El correo no tiene un formato válido.")]
        public string Email { get; set; } = string.Empty;

        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = string.Empty;
    }

    public class TokenResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}
