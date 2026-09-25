using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SistemaInventarioApi.Controllers;
using SistemaInventarioApi.DTOs;
using SistemaInventarioApi.Models;
using SistemaInventarioApi.Services;
using SistemaInventarioApi.Tests.Infrastructure;
using System.Security.Cryptography;
using Xunit;

namespace SistemaInventarioApi.Tests;

public sealed class AuthControllerTests : IAsyncLifetime
{
    private readonly SqliteTestDatabase _database = new();
    private readonly IPasswordHasher<Usuario> _passwordHasher =
        new PasswordHasher<Usuario>();

    public Task InitializeAsync() => _database.InitializeAsync();

    public Task DisposeAsync() => _database.DisposeAsync().AsTask();

    [Fact]
    public async Task Login_WhenUserIsInactive_ReturnsUnauthorizedWithoutToken()
    {
        const string email = "inactivo@example.test";
        var password = new string('x', 12);
        await SeedUsuarioAsync(email, password, activo: false);

        await using var context = _database.CreateContext();
        var controller = CreateController(context);

        var result = await controller.Login(new LoginDto
        {
            Email = email,
            Password = password
        });

        var response = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        Assert.IsType<ProblemDetails>(response.Value);
    }

    [Fact]
    public async Task Login_WhenCredentialsAreValid_ReturnsTokenWithoutPasswordHash()
    {
        const string email = "vendedor@example.test";
        var password = new string('x', 12);
        await SeedUsuarioAsync(email, password, activo: true);

        await using var context = _database.CreateContext();
        var controller = CreateController(context);

        var result = await controller.Login(new LoginDto
        {
            Email = email,
            Password = password
        });

        var response = Assert.IsType<OkObjectResult>(result.Result);
        var token = Assert.IsType<TokenResponseDto>(response.Value);

        Assert.False(string.IsNullOrWhiteSpace(token.Token));
        Assert.Equal(email, token.Email);
        Assert.Equal(RolUsuario.Vendedor.ToString(), token.Rol);
    }

    private async Task SeedUsuarioAsync(string email, string password, bool activo)
    {
        await using var context = _database.CreateContext();
        var usuario = new Usuario
        {
            Email = email,
            PasswordHash = string.Empty,
            Rol = RolUsuario.Vendedor,
            Activo = activo
        };

        usuario.PasswordHash = _passwordHasher.HashPassword(usuario, password);
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();
    }

    private AuthController CreateController(
        SistemaInventarioApi.Data.AppDbContext context)
    {
        var settings = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(48)),
            ["Jwt:Issuer"] = "tests",
            ["Jwt:Audience"] = "tests",
            ["Jwt:ExpiresMinutes"] = "60"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        return new AuthController(
            context,
            new TokenService(configuration),
            _passwordHasher)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }
}
