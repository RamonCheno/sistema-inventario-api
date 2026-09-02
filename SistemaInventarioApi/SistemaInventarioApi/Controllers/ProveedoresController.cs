using Microsoft.AspNetCore.Mvc;
using SistemaInventarioApi.Data;
using SistemaInventarioApi.DTOs;
using SistemaInventarioApi.Models;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<ProveedorDto>> GetProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return NotFound();

            return Ok(new ProveedorDto { Id = proveedor.Id, Nombre = proveedor.Nombre, Telefono = proveedor.Telefono, Email = proveedor.Email });
        }

        [HttpPost]
        public async Task<ActionResult<ProveedorDto>> CreateProveedor(CreateProveedorDto dto)
        {
            var proveedor = new Proveedor { Nombre = dto.Nombre, Telefono = dto.Telefono, Email = dto.Email };

            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            var resultado = new ProveedorDto { Id = proveedor.Id, Nombre = proveedor.Nombre, Telefono = proveedor.Telefono, Email = proveedor.Email };
            return CreatedAtAction(nameof(GetProveedor), new { id = proveedor.Id }, resultado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProveedor(int id, UpdateProveedorDto dto)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return NotFound();

            proveedor.Nombre = dto.Nombre;
            proveedor.Telefono = dto.Telefono;
            proveedor.Email = dto.Email;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return NotFound();

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
