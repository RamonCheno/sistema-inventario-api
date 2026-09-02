using System.ComponentModel.DataAnnotations;

namespace SistemaInventarioApi.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Email { get; set; }
        public string Telefono { get; set; }

        public ICollection<Venta> Ventas { get; set; }
    }
}
