namespace SistemaInventarioApi.DTOs
{
    public class ProveedorDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class CreateProveedorDto
    {
        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "El nombre del proveedor es obligatorio.")]
        [System.ComponentModel.DataAnnotations.MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "El teléfono del proveedor es obligatorio.")]
        [System.ComponentModel.DataAnnotations.MaxLength(30)]
        public string Telefono { get; set; } = string.Empty;

        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "El correo del proveedor es obligatorio.")]
        [SistemaInventarioApi.Validation.EmailFormat(
            ErrorMessage = "El correo del proveedor no tiene un formato válido.")]
        [System.ComponentModel.DataAnnotations.MaxLength(256)]
        public string Email { get; set; } = string.Empty;
    }

    public class UpdateProveedorDto
    {
        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "El nombre del proveedor es obligatorio.")]
        [System.ComponentModel.DataAnnotations.MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "El teléfono del proveedor es obligatorio.")]
        [System.ComponentModel.DataAnnotations.MaxLength(30)]
        public string Telefono { get; set; } = string.Empty;

        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "El correo del proveedor es obligatorio.")]
        [SistemaInventarioApi.Validation.EmailFormat(
            ErrorMessage = "El correo del proveedor no tiene un formato válido.")]
        [System.ComponentModel.DataAnnotations.MaxLength(256)]
        public string Email { get; set; } = string.Empty;
    }
}
