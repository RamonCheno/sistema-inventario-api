using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SistemaInventarioApi.Models
{
    public class Venta
    {
        [Key]
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        [Precision(18, 2)] 
        public decimal Total { get; set; }

        public int ClienteId { get; set; }
        //[ForeignKey("Cliente")]
        public Cliente Cliente { get; set; } = null!;

        public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
