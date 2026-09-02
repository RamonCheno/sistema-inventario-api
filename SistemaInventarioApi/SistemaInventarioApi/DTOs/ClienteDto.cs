namespace SistemaInventarioApi.DTOs
{
    public class ClienteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
    }

    public class CreateClienteDto
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
    }

    public class UpdateClienteDto
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
    }
}
