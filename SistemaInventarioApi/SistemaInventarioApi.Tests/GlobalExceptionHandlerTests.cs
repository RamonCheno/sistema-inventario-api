using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SistemaInventarioApi.Middleware;
using System.Text.Json;
using Xunit;

namespace SistemaInventarioApi.Tests;

public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsync_WhenForeignKeyConstraintFails_ReturnsConflictProblemDetails()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var handler = new GlobalExceptionHandler(
            NullLogger<GlobalExceptionHandler>.Instance);

        var handled = await handler.TryHandleAsync(
            context,
            new DbUpdateException(
                "Error al guardar.",
                new Exception("The DELETE statement conflicted with the REFERENCE constraint.")),
            CancellationToken.None);

        context.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(context.Response.Body);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
        Assert.Equal(
            "Conflicto de datos",
            document.RootElement.GetProperty("title").GetString());
    }

    [Fact]
    public async Task TryHandleAsync_WhenUnexpectedExceptionOccurs_HidesInternalDetails()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var handler = new GlobalExceptionHandler(
            NullLogger<GlobalExceptionHandler>.Instance);

        await handler.TryHandleAsync(
            context,
            new InvalidOperationException("dato-interno-que-no-debe-exponerse"),
            CancellationToken.None);

        context.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        var payload = document.RootElement.GetRawText();

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.DoesNotContain("dato-interno-que-no-debe-exponerse", payload);
    }
}
