namespace SistemaInventarioApi.DTOs
{
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int StockMinimo { get; set; }
        public int CategoriaId { get; set; }
        public int ProveedorId { get; set; }
    }

    public class CreateProductoDto
    {
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int StockMinimo { get; set; }
        public int CategoriaId { get; set; }
        public int ProveedorId { get; set; }
    }

    public class UpdateProductoDto
    {
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int StockMinimo { get; set; }
        public int CategoriaId { get; set; }
        public int ProveedorId { get; set; }
    }
}
