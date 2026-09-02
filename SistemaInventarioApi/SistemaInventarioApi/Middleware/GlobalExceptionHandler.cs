using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SistemaInventarioApi.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {

        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Excepción no controlada: {Message}", exception.Message);

            var (statusCode, title, detail) = MapException(exception);

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail
            };

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true; // ya se manejó, ASP.NET Core no debe seguir propagando
        }

        private static (int StatusCode, string Title, string Detail) MapException(Exception exception)
        {
            // Violación de FK al hacer Delete/Update (restricción Restrict que configuramos en el Paso 7)
            if (exception is DbUpdateException dbEx &&
                dbEx.InnerException?.Message.Contains("REFERENCE constraint", StringComparison.OrdinalIgnoreCase) == true)
            {
                return (StatusCodes.Status409Conflict,
                        "Conflicto de datos",
                        "No se puede completar la operación porque el registro está relacionado con otros datos existentes.");
            }

            // Cualquier otro error no controlado
            return (StatusCodes.Status500InternalServerError,
                    "Error interno del servidor",
                    "Ocurrió un error inesperado. Intenta de nuevo más tarde.");
        }

    }
}
