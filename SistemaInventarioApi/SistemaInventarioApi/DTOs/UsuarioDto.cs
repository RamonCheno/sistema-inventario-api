using System.ComponentModel.DataAnnotations;

namespace SistemaInventarioApi.DTOs
{
    public class CreateUsuarioDto
    {
        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "El correo del usuario es obligatorio.")]
        [SistemaInventarioApi.Validation.EmailFormat(
            ErrorMessage = "El correo del usuario no tiene un formato válido.")]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string Password { get; set; } = string.Empty;

        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "El rol es obligatorio.")]
        public string Rol { get; set; } = string.Empty;
    }

    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }

    public class UpdateUsuarioEstadoDto
    {
        public bool Activo { get; set; }
    }

}
