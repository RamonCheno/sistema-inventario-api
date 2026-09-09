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
        [System.ComponentModel.DataAnnotations.Range(
            1,
            int.MaxValue,
            ErrorMessage = "El producto debe ser válido.")]
        public int ProductoId { get; set; }

        [System.ComponentModel.DataAnnotations.Range(
            1,
            int.MaxValue,
            ErrorMessage = "La cantidad debe ser mayor que cero.")]
        public int Cantidad { get; set; }
    }

    public class CreateVentaDto
    {
        [System.ComponentModel.DataAnnotations.Range(
            1,
            int.MaxValue,
            ErrorMessage = "El cliente debe ser válido.")]
        public int ClienteId { get; set; }

        [System.ComponentModel.DataAnnotations.MinLength(
            1,
            ErrorMessage = "La venta debe tener al menos un detalle.")]
        public List<CreateDetalleVentaItemDto> Detalles { get; set; } = new();
    }
}
