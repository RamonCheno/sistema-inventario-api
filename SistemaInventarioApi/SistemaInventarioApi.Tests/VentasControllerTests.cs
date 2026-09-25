using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventarioApi.Controllers;
using SistemaInventarioApi.DTOs;
using SistemaInventarioApi.Models;
using SistemaInventarioApi.Tests.Infrastructure;
using Xunit;

namespace SistemaInventarioApi.Tests;

public sealed class VentasControllerTests : IAsyncLifetime
{
    private readonly SqliteTestDatabase _database = new();

    public Task InitializeAsync() => _database.InitializeAsync();

    public Task DisposeAsync() => _database.DisposeAsync().AsTask();

    [Fact]
    public async Task CreateVenta_WhenStockIsInsufficient_DoesNotPersistPartialSale()
    {
        var (clienteId, productoId) = await SeedVentaAsync(stock: 1);

        await using var context = _database.CreateContext();
        var controller = new VentasController(context);

        var result = await controller.CreateVenta(new CreateVentaDto
        {
            ClienteId = clienteId,
            Detalles = [new CreateDetalleVentaItemDto
            {
                ProductoId = productoId,
                Cantidad = 2
            }]
        });

        Assert.IsType<BadRequestObjectResult>(result.Result);

        await using var verificationContext = _database.CreateContext();
        Assert.Equal(1, await verificationContext.Productos
            .Where(producto => producto.Id == productoId)
            .Select(producto => producto.Stock)
            .SingleAsync());
        Assert.Equal(0, await verificationContext.Ventas.CountAsync());
    }

    [Fact]
    public async Task DeleteVenta_RestoresStockAndDeletesItsDetails()
    {
        var (clienteId, productoId) = await SeedVentaAsync(stock: 3);
        int ventaId;

        await using (var seedContext = _database.CreateContext())
        {
            var venta = new Venta
            {
                ClienteId = clienteId,
                Fecha = DateTime.UtcNow,
                Total = 20m,
                Detalles = [new DetalleVenta
                {
                    ProductoId = productoId,
                    Cantidad = 2,
                    PrecioUnitario = 10m
                }]
            };

            seedContext.Ventas.Add(venta);
            await seedContext.SaveChangesAsync();
            ventaId = venta.Id;
        }

        await using (var context = _database.CreateContext())
        {
            var controller = new VentasController(context);
            var result = await controller.DeleteVenta(ventaId);

            Assert.IsType<NoContentResult>(result);
        }

        await using var verificationContext = _database.CreateContext();
        Assert.Equal(5, await verificationContext.Productos
            .Where(producto => producto.Id == productoId)
            .Select(producto => producto.Stock)
            .SingleAsync());
        Assert.Equal(0, await verificationContext.Ventas.CountAsync());
        Assert.Equal(0, await verificationContext.DetalleVentas.CountAsync());
    }

    [Fact]
    public async Task CreateVenta_WhenRepeatedProductExceedsStock_RollsBackTheWholeSale()
    {
        var (clienteId, productoId) = await SeedVentaAsync(stock: 3);

        await using var context = _database.CreateContext();
        var controller = new VentasController(context);

        var result = await controller.CreateVenta(new CreateVentaDto
        {
            ClienteId = clienteId,
            Detalles =
            [
                new CreateDetalleVentaItemDto
                {
                    ProductoId = productoId,
                    Cantidad = 2
                },
                new CreateDetalleVentaItemDto
                {
                    ProductoId = productoId,
                    Cantidad = 2
                }
            ]
        });

        Assert.IsType<ConflictObjectResult>(result.Result);

        await using var verificationContext = _database.CreateContext();
        Assert.Equal(3, await verificationContext.Productos
            .Where(producto => producto.Id == productoId)
            .Select(producto => producto.Stock)
            .SingleAsync());
        Assert.Equal(0, await verificationContext.Ventas.CountAsync());
    }

    private async Task<(int ClienteId, int ProductoId)> SeedVentaAsync(int stock)
    {
        await using var context = _database.CreateContext();

        var categoria = new Categoria { Nombre = "Categoría" };
        var proveedor = new Proveedor
        {
            Nombre = "Proveedor",
            Email = "proveedor@example.test",
            Telefono = "0000000000"
        };
        var cliente = new Cliente
        {
            Nombre = "Cliente",
            Email = "cliente@example.test",
            Telefono = "0000000000"
        };

        context.AddRange(categoria, proveedor, cliente);
        await context.SaveChangesAsync();

        var producto = new Producto
        {
            Nombre = "Producto",
            Precio = 10m,
            Stock = stock,
            StockMinimo = 0,
            CategoriaId = categoria.Id,
            ProveedorId = proveedor.Id
        };

        context.Productos.Add(producto);
        await context.SaveChangesAsync();

        return (cliente.Id, producto.Id);
    }
}
