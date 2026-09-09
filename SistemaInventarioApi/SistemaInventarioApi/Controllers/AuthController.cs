using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventarioApi.Common;
using SistemaInventarioApi.Data;
using SistemaInventarioApi.DTOs;
using SistemaInventarioApi.Models;
using SistemaInventarioApi.Services;


namespace SistemaInventarioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public AuthController(
            AppDbContext context,
            TokenService tokenService,
            IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(
            LoginDto dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(
                    usuario => usuario.Email == dto.Email.Trim());

            /*
             * Se utiliza la misma respuesta para:
             * - Usuario inexistente.
             * - Usuario deshabilitado.
             * - Contraseña incorrecta.
             *
             * Esto evita revelar cuáles cuentas existen.
             */
            if (usuario is null || !usuario.Activo)
            {
                return CredencialesInvalidas();
            }

            var resultado =
                _passwordHasher.VerifyHashedPassword(
                    usuario,
                    usuario.PasswordHash,
                    dto.Password);

            if (resultado == PasswordVerificationResult.Failed)
            {
                return CredencialesInvalidas();
            }

            var (token, expiresAt) =
                _tokenService.GenerarToken(usuario);

            return Ok(new TokenResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                Email = usuario.Email,
                Rol = usuario.Rol.ToString()
            });
        }

        private UnauthorizedObjectResult CredencialesInvalidas()
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "No fue posible iniciar sesión.",
                Detail = "El correo o la contraseña son incorrectos.",
                Instance = HttpContext.Request.Path
            };

            problemDetails.Extensions["code"] =
                ErrorCodes.Auth.InvalidCredentials;

            return Unauthorized(problemDetails);
        }
    }
}
