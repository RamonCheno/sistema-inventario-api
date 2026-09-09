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
        [EmailAddress]
        [MaxLength(256)]
        public required string Email { get; set;  }

        [Required]
        [MaxLength(512)]
        public required string PasswordHash { get; set; }

        public RolUsuario Rol { get; set; }

        public bool Activo { get; set; } = true;

    }

    public static class Roles
    {
        public const string Administrador = nameof(RolUsuario.Administrador);
        public const string Vendedor = nameof(RolUsuario.Vendedor);
        public const string Almacenista = nameof(RolUsuario.Almacenista);
        public const string GestionInventario =
        Administrador + "," + Almacenista;

        public const string GestionComercial = Administrador + "," + Vendedor;

    }
}
