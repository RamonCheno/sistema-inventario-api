using Microsoft.AspNetCore.Mvc;
using SistemaInventarioApi.Data;
using SistemaInventarioApi.DTOs;
using SistemaInventarioApi.Models;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SistemaInventarioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/categorias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
        {
            var categorias = await _context.Categorias
                .Select(c => new CategoriaDto { Id = c.Id, Nombre = c.Nombre })
                .ToListAsync();

            return Ok(categorias);
        }

        // GET: api/categorias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return NotFound();

            return Ok(new CategoriaDto { Id = categoria.Id, Nombre = categoria.Nombre });
        }

        // POST: api/categorias
        [HttpPost]
        public async Task<ActionResult<CategoriaDto>> CreateCategoria(CreateCategoriaDto dto)
        {
            var categoria = new Categoria { Nombre = dto.Nombre };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            var resultado = new CategoriaDto { Id = categoria.Id, Nombre = categoria.Nombre };

            return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, resultado);
        }

        // PUT: api/categorias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoria(int id, UpdateCategoriaDto dto)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return NotFound();

            categoria.Nombre = dto.Nombre;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/categorias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return NotFound();

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
