namespace SistemaInventarioApi.DTOs
{
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int StockMinimo { get; set; }
        public int CategoriaId { get; set; }
        public int ProveedorId { get; set; }
    }

    public class CreateProductoDto
    {
        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "El nombre del producto es obligatorio.")]
        [System.ComponentModel.DataAnnotations.MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Range(
            typeof(decimal),
            "0.01",
            "79228162514264337593543950335",
            ErrorMessage = "El precio debe ser mayor que cero.")]
        public decimal Precio { get; set; }

        [System.ComponentModel.DataAnnotations.Range(
            0,
            int.MaxValue,
            ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        [System.ComponentModel.DataAnnotations.Range(
            0,
            int.MaxValue,
            ErrorMessage = "El stock mínimo no puede ser negativo.")]
        public int StockMinimo { get; set; }

        [System.ComponentModel.DataAnnotations.Range(
            1,
            int.MaxValue,
            ErrorMessage = "La categoría debe ser válida.")]
        public int CategoriaId { get; set; }

        [System.ComponentModel.DataAnnotations.Range(
            1,
            int.MaxValue,
            ErrorMessage = "El proveedor debe ser válido.")]
        public int ProveedorId { get; set; }
    }

    public class UpdateProductoDto
    {
        [SistemaInventarioApi.Validation.NotWhiteSpace(
            ErrorMessage = "El nombre del producto es obligatorio.")]
        [System.ComponentModel.DataAnnotations.MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Range(
            typeof(decimal),
            "0.01",
            "79228162514264337593543950335",
            ErrorMessage = "El precio debe ser mayor que cero.")]
        public decimal Precio { get; set; }

        [System.ComponentModel.DataAnnotations.Range(
            0,
            int.MaxValue,
            ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        [System.ComponentModel.DataAnnotations.Range(
            0,
            int.MaxValue,
            ErrorMessage = "El stock mínimo no puede ser negativo.")]
        public int StockMinimo { get; set; }

        [System.ComponentModel.DataAnnotations.Range(
            1,
            int.MaxValue,
            ErrorMessage = "La categoría debe ser válida.")]
        public int CategoriaId { get; set; }

        [System.ComponentModel.DataAnnotations.Range(
            1,
            int.MaxValue,
            ErrorMessage = "El proveedor debe ser válido.")]
        public int ProveedorId { get; set; }
    }
}
