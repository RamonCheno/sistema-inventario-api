# Sistema de Inventario y Ventas — API REST

Backend REST para administrar inventario, clientes, ventas y usuarios. Implementa reglas de negocio,
persistencia relacional, autenticación JWT y autorización por roles para una aplicación full stack
orientada a portafolio.

Forma parte del repositorio orquestador
[sistema-inventario](https://github.com/RamonCheno/sistema-inventario), aunque también puede
ejecutarse y versionarse de manera independiente.

## Stack tecnológico

| Área | Tecnología |
|---|---|
| Plataforma | .NET 10, ASP.NET Core Web API |
| Lenguaje | C# |
| Persistencia | Entity Framework Core 10 |
| Base de datos | SQL Server LocalDB / SQL Server 2022 |
| Seguridad | JWT Bearer, `PasswordHasher<TUser>`, autorización por roles |
| Contrato HTTP | DTOs, `ProblemDetails`, `ValidationProblemDetails` planificado |
| Documentación | OpenAPI, Scalar, Swashbuckle |
| Infraestructura | Docker y Docker Compose |
| Pruebas manuales | Bruno |

## Arquitectura

```text
HTTP Request
     │
     ▼
Controllers + DTOs
     │ autenticación, autorización y validación
     ▼
Reglas de negocio
     │
     ▼
AppDbContext + EF Core
     │
     ▼
SQL Server
```

La API utiliza DTOs para evitar exponer directamente entidades persistentes, contraseñas o
navegaciones de EF Core. El manejador global transforma excepciones en respuestas HTTP controladas.

## Funcionalidades

- CRUD de categorías, proveedores, productos y clientes.
- Ventas con detalle, cálculo de total y descuento de stock.
- Consulta de ventas y eliminación exclusiva para administradores.
- Login con JWT.
- Listado, creación, habilitación, deshabilitación y eliminación de usuarios.
- Rechazo inmediato de tokens pertenecientes a usuarios inactivos o eliminados.
- Migraciones tipadas de EF Core.
- Documentación interactiva de endpoints en Development.

## Roles y permisos

| Rol | Permisos principales |
|---|---|
| Administrador | Acceso completo, gestión de usuarios y eliminación de ventas |
| Almacenista | Gestión de categorías, proveedores y productos |
| Vendedor | Gestión de clientes y ventas; consulta de inventario |

La autorización siempre se valida en el servidor. El frontend no se considera una frontera de
seguridad.

## Seguridad

- Autenticación global, excepto `POST /api/Auth/login`.
- Claims de identidad y rol incluidos en el JWT.
- Comprobación de existencia y estado activo mediante `OnTokenValidated`.
- Contraseñas almacenadas como hash y nunca incluidas en respuestas.
- Protección contra autoeliminación y autodeshabilitación administrativa.
- CORS limitado a orígenes configurados.
- Secretos externos a Git y a los archivos de documentación.
- Errores globales sin stack trace en las respuestas.

Está pendiente completar la fase GREEN de validación: DTOs con reglas tipadas, normalización de
texto y mensajes propios en español mediante un contrato consistente.

## Configuración

Variables principales:

```text
ConnectionStrings__SistemaInventarioDB
Cors__AllowedOrigins__0
Jwt__Key
Jwt__Issuer
Jwt__Audience
Jwt__ExpiresMinutes
Admin__Email
Admin__Password
RUN_MIGRATIONS
```

Las claves JWT, contraseñas iniciales y cadenas de producción deben suministrarse mediante variables
de entorno o un gestor de secretos.

## Ejecución local

Requisitos:

- .NET SDK 10.
- SQL Server LocalDB.
- `dotnet-ef` compatible con EF Core 10.

Desde `SistemaInventarioApi/SistemaInventarioApi`:

```powershell
dotnet restore
dotnet ef database update
dotnet run --launch-profile http
```

- API: `http://localhost:5108`
- Scalar: `http://localhost:5108/scalar/v1`
- OpenAPI: `http://localhost:5108/openapi/v1.json`

## Migraciones

```powershell
dotnet ef migrations add NombreMigracion
dotnet ef database update
```

Antes de aplicar una migración se revisan `Up`, `Down` y `ModelSnapshot`. No se modifican migraciones
ya aplicadas ni se introduce SQL plano mientras exista una alternativa segura con EF Core.

## Estado del proyecto

La API, autenticación, roles, Docker y colección Bruno están implementados. Pendientes principales:

- Fase GREEN de validación de entradas.
- Pruebas unitarias y de integración automatizadas.
- Identificadores públicos UUID por etapas.
- CI/CD y despliegue público.

Cuando se trabaja desde el orquestador, consultar su `AGENTS.md` para reglas de colaboración,
seguridad y verificación.

## Despliegue

### Publicación local

```powershell
dotnet publish -c Release
```

### Docker

La opción recomendada es levantar API, UI y SQL Server desde el repositorio orquestador:

```powershell
docker compose up --build -d
```

La API queda expuesta en `http://localhost:8080`. En Compose se ejecuta como Production, por lo que
Scalar no está publicado con la configuración actual.

## Pruebas

La colección Bruno se encuentra en el repositorio orquestador:

```text
Pruebas/Bruno/Sistema Inventario API
```

Incluye:

- Autenticación y roles.
- CRUD completo.
- Códigos `400`, `401`, `403`, `404` y `409`.
- Reglas de stock y ventas.
- Usuarios inactivos o eliminados.
- Casos RED para validaciones pendientes.
- Limpieza respetando dependencias.

Verificación técnica:

```powershell
dotnet build
dotnet publish -c Release
```

No debe afirmarse que una prueba pasó si no fue ejecutada. La suite RED no debe utilizarse sobre una
base con información importante mientras los endpoints todavía puedan persistir entradas inválidas.
