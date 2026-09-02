namespace SistemaInventarioApi.DTOs
{
    public class DetalleVentaDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }

    public class VentaDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public int ClienteId { get; set; }
        public List<DetalleVentaDto> Detalles { get; set; } = new();
    }

    public class CreateDetalleVentaItemDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }

    public class CreateVentaDto
    {
        public int ClienteId { get; set; }
        public List<CreateDetalleVentaItemDto> Detalles { get; set; } = new();
    }
}
