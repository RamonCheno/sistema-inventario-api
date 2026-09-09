using System.ComponentModel.DataAnnotations;

namespace SistemaInventarioApi.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    }
}
