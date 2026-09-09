using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SistemaInventarioApi.Common;
using SistemaInventarioApi.Data;
using SistemaInventarioApi.Middleware;
using SistemaInventarioApi.Models;
using SistemaInventarioApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Los controladores están protegidos por defecto.
// [AllowAnonymous] permite excepciones, como POST /api/Auth/login.
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});

// Personaliza las respuestas producidas automáticamente
// por las validaciones de los DTO.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        if (context.ModelState.TryGetValue("id", out var idState)
            && idState.Errors.Count > 0)
        {
            context.ModelState.Remove("id");
            context.ModelState.AddModelError(
                "id",
                "El parámetro 'id' debe ser un entero positivo.");
        }

        var problemDetails = new ValidationProblemDetails(
            context.ModelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Datos de entrada inválidos.",
            Detail =
                "Corrige los campos indicados e intenta nuevamente.",
            Instance = context.HttpContext.Request.Path
        };

        problemDetails.Extensions["code"] =
            ErrorCodes.ValidationFailed;

        return new BadRequestObjectResult(problemDetails);
    };
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(
    options => options.AddScalarFilters());

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "SistemaInventarioDB")));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var allowedOrigins =
    builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>()
    ?? ["http://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Servicios de autenticación.
builder.Services.AddSingleton<
    IPasswordHasher<Usuario>,
    PasswordHasher<Usuario>>();

builder.Services.AddScoped<TokenService>();

var jwtSection = builder.Configuration.GetSection("Jwt");

var jwtKey = jwtSection["Key"]
    ?? throw new InvalidOperationException(
        "No se configuró Jwt:Key.");

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSection["Issuer"],
                ValidAudience = jwtSection["Audience"],

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)),

                ClockSkew = TimeSpan.Zero
            };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userIdClaim = context.Principal?
                    .FindFirstValue(
                        ClaimTypes.NameIdentifier);

                if (!int.TryParse(
                        userIdClaim,
                        out var userId))
                {
                    context.Fail(
                        "Token sin identificador de usuario.");

                    return;
                }

                var db = context.HttpContext
                    .RequestServices
                    .GetRequiredService<AppDbContext>();

                var usuarioActivo = await db.Usuarios
                    .AsNoTracking()
                    .AnyAsync(
                        usuario =>
                            usuario.Id == userId
                            && usuario.Activo);

                if (!usuarioActivo)
                {
                    context.Fail(
                        "El usuario está inactivo o no existe.");
                }
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Scalar está disponible en desarrollo o cuando
// se habilita explícitamente.
var scalarEnabled =
    app.Environment.IsDevelopment()
    || app.Configuration.GetValue<bool>(
        "Scalar:Enabled");

if (scalarEnabled)
{
    app.MapSwagger(
        "/openapi/{documentName}.json");

    app.MapScalarApiReference("/scalar");
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseCors("PermitirFrontend");

// Primero se identifica al usuario y después se autoriza.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// En Docker, aplica automáticamente
// las migraciones pendientes.
if (Environment.GetEnvironmentVariable(
        "RUN_MIGRATIONS") == "true")
{
    using var migrationScope =
        app.Services.CreateScope();

    var db = migrationScope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    await db.Database.MigrateAsync();
}

// Crea el administrador inicial si todavía no existe uno.
using (var seedScope = app.Services.CreateScope())
{
    var db = seedScope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    var adminEmail =
        builder.Configuration["Admin:Email"]
        ?? throw new InvalidOperationException(
            "No se configuró Admin:Email.");

    // Corrección temporal para el administrador afectado
    // por la migración AgregarEstadoUsuario.
    var administradorExistente =
        await db.Usuarios.FirstOrDefaultAsync(
            usuario => usuario.Email == adminEmail);

    if (administradorExistente is not null
        && !administradorExistente.Activo)
    {
        administradorExistente.Activo = true;
        await db.SaveChangesAsync();
    }

    var existeAdministrador =
        await db.Usuarios.AnyAsync(
            usuario =>
                usuario.Rol ==
                RolUsuario.Administrador);

    if (!existeAdministrador)
    {
        var adminPassword =
            builder.Configuration["Admin:Password"]
            ?? throw new InvalidOperationException(
                "No se configuró Admin:Password.");

        var passwordHasher =
            seedScope.ServiceProvider
                .GetRequiredService<
                    IPasswordHasher<Usuario>>();

        var administrador = new Usuario
        {
            Email = adminEmail,
            PasswordHash = string.Empty,
            Rol = RolUsuario.Administrador,
            Activo = true
        };

        administrador.PasswordHash =
            passwordHasher.HashPassword(
                administrador,
                adminPassword);

        db.Usuarios.Add(administrador);
        await db.SaveChangesAsync();
    }
}

app.Run();
