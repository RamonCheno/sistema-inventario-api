namespace SistemaInventarioApi.DTOs
{
    public class ClienteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
    }

    public class CreateClienteDto
    {
        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "El nombre del cliente es obligatorio.")]
        [System.ComponentModel.DataAnnotations.MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "El correo del cliente es obligatorio.")]
        [SistemaInventarioApi.Validation.EmailFormat(
            ErrorMessage = "El correo del cliente no tiene un formato válido.")]
        [System.ComponentModel.DataAnnotations.MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "El teléfono del cliente es obligatorio.")]
        [System.ComponentModel.DataAnnotations.MaxLength(30)]
        public string Telefono { get; set; } = string.Empty;
    }

    public class UpdateClienteDto
    {
        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "El nombre del cliente es obligatorio.")]
        [System.ComponentModel.DataAnnotations.MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "El correo del cliente es obligatorio.")]
        [SistemaInventarioApi.Validation.EmailFormat(
            ErrorMessage = "El correo del cliente no tiene un formato válido.")]
        [System.ComponentModel.DataAnnotations.MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "El teléfono del cliente es obligatorio.")]
        [System.ComponentModel.DataAnnotations.MaxLength(30)]
        public string Telefono { get; set; } = string.Empty;
    }
}
