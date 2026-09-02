using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SistemaInventarioApi.Models
{
    public class DetalleVenta
    {
        [Key]
        public int Id { get; set; }

        public int Cantidad { get; set; }
        [Precision(18, 2)]
        public decimal PrecioUnitario { get; set; }

        public int VentaId { get; set; }
        //[ForeignKey("Venta")]
        public Venta Venta { get; set; }

        public int ProductoId { get; set; }
        //[ForeignKey("Producto")] 
        public Producto Producto { get; set; }
    }
}
