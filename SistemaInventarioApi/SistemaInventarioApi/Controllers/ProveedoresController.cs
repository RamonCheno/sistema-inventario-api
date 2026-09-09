using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventarioApi.Data;
using SistemaInventarioApi.DTOs;
using SistemaInventarioApi.Models;

namespace SistemaInventarioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProveedoresController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProveedorDto>>> GetProveedores()
        {
            var proveedores = await _context.Proveedores
                .Select(p => new ProveedorDto { Id = p.Id, Nombre = p.Nombre, Telefono = p.Telefono, Email = p.Email })
                .ToListAsync();

            return Ok(proveedores);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProveedorDto>> GetProveedor(
            [SistemaInventarioApi.Validation.PositiveId] int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return NotFound();

            return Ok(new ProveedorDto { Id = proveedor.Id, Nombre = proveedor.Nombre, Telefono = proveedor.Telefono, Email = proveedor.Email });
        }

        [HttpPost]
        [Authorize(Roles = Roles.GestionInventario)]

        public async Task<ActionResult<ProveedorDto>> CreateProveedor(CreateProveedorDto dto)
        {
            var proveedor = new Proveedor
            {
                Nombre = dto.Nombre.Trim(),
                Telefono = dto.Telefono.Trim(),
                Email = dto.Email.Trim()
            };

            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            var resultado = new ProveedorDto { Id = proveedor.Id, Nombre = proveedor.Nombre, Telefono = proveedor.Telefono, Email = proveedor.Email };
            return CreatedAtAction(nameof(GetProveedor), new { id = proveedor.Id }, resultado);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.GestionInventario)]

        public async Task<IActionResult> UpdateProveedor(
            [SistemaInventarioApi.Validation.PositiveId] int id,
            UpdateProveedorDto dto)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return NotFound();

            proveedor.Nombre = dto.Nombre.Trim();
            proveedor.Telefono = dto.Telefono.Trim();
            proveedor.Email = dto.Email.Trim();
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.GestionInventario)]
        public async Task<IActionResult> DeleteProveedor(
            [SistemaInventarioApi.Validation.PositiveId] int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return NotFound();

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
