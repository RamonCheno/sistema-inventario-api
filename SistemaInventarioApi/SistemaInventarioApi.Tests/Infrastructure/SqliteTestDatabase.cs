using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SistemaInventarioApi.Data;

namespace SistemaInventarioApi.Tests.Infrastructure;

public sealed class SqliteTestDatabase : IAsyncDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private DbContextOptions<AppDbContext>? _options;

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
    }

    public AppDbContext CreateContext()
    {
        return new AppDbContext(
            _options ?? throw new InvalidOperationException(
                "La base de pruebas no se ha inicializado."));
    }

    public ValueTask DisposeAsync() => _connection.DisposeAsync();
}
