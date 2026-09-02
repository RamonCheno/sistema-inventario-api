using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventarioApi.Data;
using SistemaInventarioApi.DTOs;
using SistemaInventarioApi.Models;

namespace SistemaInventarioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes()
        {
            var clientes = await _context.Clientes
                .Select(c => new ClienteDto { Id = c.Id, Nombre = c.Nombre, Email = c.Email, Telefono = c.Telefono })
                .ToListAsync();

            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> GetCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            return Ok(new ClienteDto { Id = cliente.Id, Nombre = cliente.Nombre, Email = cliente.Email, Telefono = cliente.Telefono });
        }

        [HttpPost]
        public async Task<ActionResult<ClienteDto>> CreateCliente(CreateClienteDto dto)
        {
            var cliente = new Cliente { Nombre = dto.Nombre, Email = dto.Email, Telefono = dto.Telefono };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            var resultado = new ClienteDto { Id = cliente.Id, Nombre = cliente.Nombre, Email = cliente.Email, Telefono = cliente.Telefono };
            return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, resultado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, UpdateClienteDto dto)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            cliente.Nombre = dto.Nombre;
            cliente.Email = dto.Email;
            cliente.Telefono = dto.Telefono;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
