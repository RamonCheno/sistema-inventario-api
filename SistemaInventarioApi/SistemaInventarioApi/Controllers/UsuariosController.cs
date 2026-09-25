using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SistemaInventarioApi.Data;
using SistemaInventarioApi.DTOs;
using SistemaInventarioApi.Models;
using System.Security.Claims;
using System.Data;

namespace SistemaInventarioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Administrador)]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public UsuariosController(AppDbContext context, IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .AsNoTracking()
                .OrderBy(u => u.Email)
                .Select(u => new UsuarioDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Rol = u.Rol.ToString(),
                    Activo = u.Activo
                })
                .ToListAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDto>> GetUsuario(
            [SistemaInventarioApi.Validation.PositiveId] int id)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UsuarioDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Rol = u.Rol.ToString(),
                    Activo = u.Activo
                })
                .FirstOrDefaultAsync();
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        [HttpPost]
        public async Task<ActionResult<UsuarioDto>> CreateUsuario(
            CreateUsuarioDto dto)
        {
            if (!Enum.TryParse<RolUsuario>(
                    dto.Rol.Trim(),
                    ignoreCase: true,
                    out var rol))
            {
                return BadRequest(
                    $"Rol inválido. Usa: " +
                    $"{string.Join(", ", Enum.GetNames<RolUsuario>())}.");
            }

            var email = dto.Email.Trim();

            // Validación rápida para dar feedback inmediato en el caso
            // común. La defensa real contra la condición de carrera
            // (dos altas concurrentes con el mismo email) es el índice
            // único de la base de datos, capturado más abajo.
            var existe = await _context.Usuarios
                .AnyAsync(u => u.Email == email);

            if (existe)
                return Conflict("Ya existe un usuario con ese email.");

            var usuario = new Usuario
            {
                Email = email,
                Rol = rol,
                PasswordHash = string.Empty,
                Activo = true
            };

            usuario.PasswordHash =
                _passwordHasher.HashPassword(usuario, dto.Password);

            _context.Usuarios.Add(usuario);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (EsViolacionDeEmailUnico(ex))
            {
                return Conflict("Ya existe un usuario con ese email.");
            }

            var resultado = MapUsuario(usuario);

            return CreatedAtAction(
                nameof(GetUsuario),
                new { id = usuario.Id },
                resultado);
        }

        [HttpPatch("{id}/estado")]
        public async Task<ActionResult<UsuarioDto>> UpdateEstado(
           [SistemaInventarioApi.Validation.PositiveId] int id,
           UpdateUsuarioEstadoDto dto)
        {
            await using var transaction = await _context.Database
                .BeginTransactionAsync(IsolationLevel.Serializable);

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound();

            var usuarioActualId = ObtenerUsuarioActualId();

            if (id == usuarioActualId && !dto.Activo)
            {
                return BadRequest(
                    "No puedes deshabilitar tu propia cuenta.");
            }

            if (usuario.Rol == RolUsuario.Administrador &&
                usuario.Activo &&
                !dto.Activo)
            {
                if (await EsElUltimoAdministradorActivoAsync())
                {
                    return BadRequest(
                        "No puedes deshabilitar al último Administrador activo.");
                }
            }

            usuario.Activo = dto.Activo;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(MapUsuario(usuario));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(
            [SistemaInventarioApi.Validation.PositiveId] int id)
        {
            await using var transaction = await _context.Database
                .BeginTransactionAsync(IsolationLevel.Serializable);

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound();

            var usuarioActualId = ObtenerUsuarioActualId();

            if (id == usuarioActualId)
            {
                return BadRequest(
                    "No puedes eliminar tu propia cuenta.");
            }

            if (usuario.Rol == RolUsuario.Administrador &&
                usuario.Activo &&
                await EsElUltimoAdministradorActivoAsync())
            {
                return BadRequest(
                    "No puedes eliminar al último Administrador activo.");
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return NoContent();
        }

        private int? ObtenerUsuarioActualId()
        {
            var claim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out var id) ? id : null;
        }

        // Códigos nativos de SQL Server para violación de índice/
        // restricción única (2601: índice único, 2627: restricción
        // única o de clave primaria).
        private static bool EsViolacionDeEmailUnico(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sqlEx
                && (sqlEx.Number == 2601 || sqlEx.Number == 2627);
        }

        private async Task<bool> EsElUltimoAdministradorActivoAsync()
        {
            var administradoresActivos = await _context.Usuarios
                .CountAsync(usuario =>
                    usuario.Rol == RolUsuario.Administrador &&
                    usuario.Activo);

            return administradoresActivos <= 1;
        }

        private static UsuarioDto MapUsuario(Usuario usuario)
        {
            return new UsuarioDto
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Rol = usuario.Rol.ToString(),
                Activo = usuario.Activo
            };
        }

    }
}
