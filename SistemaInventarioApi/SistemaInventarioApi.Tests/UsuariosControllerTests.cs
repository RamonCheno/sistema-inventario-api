using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventarioApi.Controllers;
using SistemaInventarioApi.DTOs;
using SistemaInventarioApi.Models;
using SistemaInventarioApi.Tests.Infrastructure;
using System.Security.Claims;
using Xunit;

namespace SistemaInventarioApi.Tests;

public sealed class UsuariosControllerTests : IAsyncLifetime
{
    private readonly SqliteTestDatabase _database = new();
    private readonly IPasswordHasher<Usuario> _passwordHasher =
        new PasswordHasher<Usuario>();

    public Task InitializeAsync() => _database.InitializeAsync();

    public Task DisposeAsync() => _database.DisposeAsync().AsTask();

    [Fact]
    public async Task UpdateEstado_WhenOnlyOneAdminIsActive_RejectsDisablingIt()
    {
        var activeAdminId = await SeedAdministradoresAsync();

        await using var context = _database.CreateContext();
        var controller = CreateController(context);

        var result = await controller.UpdateEstado(
            activeAdminId,
            new UpdateUsuarioEstadoDto { Activo = false });

        Assert.IsType<BadRequestObjectResult>(result.Result);

        await using var verificationContext = _database.CreateContext();
        Assert.True(await verificationContext.Usuarios
            .Where(usuario => usuario.Id == activeAdminId)
            .Select(usuario => usuario.Activo)
            .SingleAsync());
    }

    [Fact]
    public async Task DeleteUsuario_WhenOnlyOneAdminIsActive_RejectsDeletingIt()
    {
        var activeAdminId = await SeedAdministradoresAsync();

        await using var context = _database.CreateContext();
        var controller = CreateController(context);

        var result = await controller.DeleteUsuario(activeAdminId);

        Assert.IsType<BadRequestObjectResult>(result);

        await using var verificationContext = _database.CreateContext();
        Assert.Equal(1, await verificationContext.Usuarios.CountAsync(usuario =>
            usuario.Rol == RolUsuario.Administrador && usuario.Activo));
    }

    [Fact]
    public async Task GetUsuarios_DoesNotExposePasswordHashesOrTrackReadEntities()
    {
        await SeedAdministradoresAsync();

        await using var context = _database.CreateContext();
        var controller = CreateController(context);

        var result = await controller.GetUsuarios();

        var response = Assert.IsType<OkObjectResult>(result.Result);
        var usuarios = Assert.IsAssignableFrom<IEnumerable<UsuarioDto>>(response.Value);

        Assert.All(usuarios, usuario => Assert.False(
            string.IsNullOrWhiteSpace(usuario.Email)));
        Assert.Empty(context.ChangeTracker.Entries());
    }

    private async Task<int> SeedAdministradoresAsync()
    {
        await using var context = _database.CreateContext();

        var activeAdmin = new Usuario
        {
            Email = "activo@example.test",
            PasswordHash = "hash",
            Rol = RolUsuario.Administrador,
            Activo = true
        };
        var inactiveAdmin = new Usuario
        {
            Email = "inactivo@example.test",
            PasswordHash = "hash",
            Rol = RolUsuario.Administrador,
            Activo = false
        };

        context.Usuarios.AddRange(activeAdmin, inactiveAdmin);
        await context.SaveChangesAsync();

        return activeAdmin.Id;
    }

    private UsuariosController CreateController(
        SistemaInventarioApi.Data.AppDbContext context)
    {
        var controller = new UsuariosController(context, _passwordHasher);
        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, "999")],
            authenticationType: "Test");

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };

        return controller;
    }
}
