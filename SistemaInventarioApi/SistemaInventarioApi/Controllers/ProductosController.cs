using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventarioApi.Data;
using SistemaInventarioApi.DTOs;
using SistemaInventarioApi.Models;

namespace SistemaInventarioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductos()
        {
            var productos = await _context.Productos
                .Select(p => new ProductoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    StockMinimo = p.StockMinimo,
                    CategoriaId = p.CategoriaId,
                    ProveedorId = p.ProveedorId
                })
                .ToListAsync();

            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            return Ok(new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Stock = producto.Stock,
                StockMinimo = producto.StockMinimo,
                CategoriaId = producto.CategoriaId,
                ProveedorId = producto.ProveedorId
            });
        }

        [HttpPost]
        public async Task<ActionResult<ProductoDto>> CreateProducto(CreateProductoDto dto)
        {
            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
            if (!categoriaExiste) return BadRequest($"No existe la categoría con Id {dto.CategoriaId}.");

            var proveedorExiste = await _context.Proveedores.AnyAsync(p => p.Id == dto.ProveedorId);
            if (!proveedorExiste) return BadRequest($"No existe el proveedor con Id {dto.ProveedorId}.");

            var producto = new Producto
            {
                Nombre = dto.Nombre,
                Precio = dto.Precio,
                Stock = dto.Stock,
                StockMinimo = dto.StockMinimo,
                CategoriaId = dto.CategoriaId,
                ProveedorId = dto.ProveedorId
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            var resultado = new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Stock = producto.Stock,
                StockMinimo = producto.StockMinimo,
                CategoriaId = producto.CategoriaId,
                ProveedorId = producto.ProveedorId
            };

            return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, resultado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProducto(int id, UpdateProductoDto dto)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
            if (!categoriaExiste) return BadRequest($"No existe la categoría con Id {dto.CategoriaId}.");

            var proveedorExiste = await _context.Proveedores.AnyAsync(p => p.Id == dto.ProveedorId);
            if (!proveedorExiste) return BadRequest($"No existe el proveedor con Id {dto.ProveedorId}.");

            producto.Nombre = dto.Nombre;
            producto.Precio = dto.Precio;
            producto.Stock = dto.Stock;
            producto.StockMinimo = dto.StockMinimo;
            producto.CategoriaId = dto.CategoriaId;
            producto.ProveedorId = dto.ProveedorId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
