using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace SistemaInventarioApi.Models
{
    public enum RolUsuario
    {
        Administrador,
        Vendedor,
        Almacenista
    }
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Email { get; set;  }

        [Required]
        public required string PasswordHash { get; set; }

        public RolUsuario Rol { get; set; }

    }
}
