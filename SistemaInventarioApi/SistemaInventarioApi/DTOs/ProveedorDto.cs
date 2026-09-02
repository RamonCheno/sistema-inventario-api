namespace SistemaInventarioApi.DTOs
{
    public class ProveedorDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
    }

    public class CreateProveedorDto
    {
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
    }

    public class UpdateProveedorDto
    {
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
    }
}
