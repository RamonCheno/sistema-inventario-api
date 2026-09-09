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
        public async Task<ActionResult<ClienteDto>> GetCliente(
            [SistemaInventarioApi.Validation.PositiveId] int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            return Ok(new ClienteDto { Id = cliente.Id, Nombre = cliente.Nombre, Email = cliente.Email, Telefono = cliente.Telefono });
        }

        [HttpPost]
        public async Task<ActionResult<ClienteDto>> CreateCliente(CreateClienteDto dto)
        {
            var cliente = new Cliente
            {
                Nombre = dto.Nombre.Trim(),
                Email = dto.Email.Trim(),
                Telefono = dto.Telefono.Trim()
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            var resultado = new ClienteDto { Id = cliente.Id, Nombre = cliente.Nombre, Email = cliente.Email, Telefono = cliente.Telefono };
            return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, resultado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(
            [SistemaInventarioApi.Validation.PositiveId] int id,
            UpdateClienteDto dto)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            cliente.Nombre = dto.Nombre.Trim();
            cliente.Email = dto.Email.Trim();
            cliente.Telefono = dto.Telefono.Trim();
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(
            [SistemaInventarioApi.Validation.PositiveId] int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
