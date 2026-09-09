using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventarioApi.Data;
using SistemaInventarioApi.DTOs;
using SistemaInventarioApi.Models;

namespace SistemaInventarioApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = Roles.GestionComercial)]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VentaDto>>> GetVentas()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Detalles)
                .Select(v => new VentaDto
                {
                    Id = v.Id,
                    Fecha = v.Fecha,
                    Total = v.Total,
                    ClienteId = v.ClienteId,
                    Detalles = v.Detalles.Select(d => new DetalleVentaDto
                    {
                        Id = d.Id,
                        ProductoId = d.ProductoId,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario
                    }).ToList()
                })
                .ToListAsync();

            return Ok(ventas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VentaDto>> GetVenta(
            [SistemaInventarioApi.Validation.PositiveId] int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null) return NotFound();

            return Ok(new VentaDto
            {
                Id = venta.Id,
                Fecha = venta.Fecha,
                Total = venta.Total,
                ClienteId = venta.ClienteId,
                Detalles = venta.Detalles.Select(d => new DetalleVentaDto
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList()
            });
        }

        [HttpPost]
        public async Task<ActionResult<VentaDto>> CreateVenta(CreateVentaDto dto)
        {
            if (dto.Detalles == null || dto.Detalles.Count == 0)
                return BadRequest("La venta debe tener al menos un detalle.");

            var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == dto.ClienteId);
            if (!clienteExiste) return BadRequest($"No existe el cliente con Id {dto.ClienteId}.");

            var venta = new Venta
            {
                Fecha = DateTime.UtcNow,
                ClienteId = dto.ClienteId,
                Detalles = new List<DetalleVenta>()
            };

            decimal total = 0;

            foreach (var item in dto.Detalles)
            {
                var producto = await _context.Productos.FindAsync(item.ProductoId);
                if (producto == null)
                    return BadRequest($"No existe el producto con Id {item.ProductoId}.");

                if (producto.Stock < item.Cantidad)
                    return BadRequest($"Stock insuficiente para '{producto.Nombre}' (disponible: {producto.Stock}, solicitado: {item.Cantidad}).");

                producto.Stock -= item.Cantidad;

                var detalle = new DetalleVenta
                {
                    ProductoId = producto.Id,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = producto.Precio
                };

                venta.Detalles.Add(detalle);
                total += detalle.Cantidad * detalle.PrecioUnitario;
            }

            venta.Total = total;

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            var resultado = new VentaDto
            {
                Id = venta.Id,
                Fecha = venta.Fecha,
                Total = venta.Total,
                ClienteId = venta.ClienteId,
                Detalles = venta.Detalles.Select(d => new DetalleVentaDto
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList()
            };

            return CreatedAtAction(nameof(GetVenta), new { id = venta.Id }, resultado);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Administrador)]
        public async Task<IActionResult> DeleteVenta(
            [SistemaInventarioApi.Validation.PositiveId] int id)
        {
            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null) return NotFound();

            _context.Ventas.Remove(venta);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
