namespace SistemaInventarioApi.DTOs
{
    public class CreateUsuarioDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
    }

    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; } = string.Empty;
    }

}
