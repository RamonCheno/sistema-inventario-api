using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SistemaInventarioApi.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Precision(18, 2)] 
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int StockMinimo { get; set; }

        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;

        public int ProveedorId { get; set; }
        public Proveedor Proveedor { get; set; } = null!;
    }
}
