namespace SistemaInventarioApi.DTOs
{
    public class CategoriaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class CreateCategoriaDto
    {
        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [System.ComponentModel.DataAnnotations.MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;
    }

    public class UpdateCategoriaDto
    {
        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [System.ComponentModel.DataAnnotations.MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;
    }
}
