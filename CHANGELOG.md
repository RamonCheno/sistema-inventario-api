# Changelog

Todos los cambios relevantes de `SistemaInventarioApi` se documentarán en este archivo.

El formato está basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.1.0/) y el proyecto
adoptará [Versionado Semántico](https://semver.org/lang/es/) cuando se publique su primera versión.

## [Unreleased]

### Agregado

- Autenticación mediante JWT Bearer con emisor, audiencia, expiración y clave de firma configurables.
- Roles `Administrador`, `Vendedor` y `Almacenista`, con políticas de autorización aplicadas en la
  API según las responsabilidades de cada perfil.
- Servicio de generación de tokens con los claims de identidad, correo y rol del usuario.
- Endpoints de administración de usuarios para crear, listar, consultar, habilitar, deshabilitar y
  eliminar cuentas.
- Validación del estado del usuario durante cada autenticación: los tokens pertenecientes a cuentas
  eliminadas o deshabilitadas dejan de ser aceptados.
- Catálogo centralizado de códigos simbólicos de error para mantener estable el contrato consumido
  por clientes web y móviles.
- Respuestas de validación globales mediante `ValidationProblemDetails` y el código
  `VALIDATION_FAILED`.
- Validadores reutilizables para cadenas compuestas solo por espacios, correos electrónicos e
  identificadores positivos.
- Validaciones tipadas en los DTOs de autenticación, usuarios, categorías, proveedores, productos,
  clientes y ventas.
- Migraciones de EF Core para crear la entidad `Usuario`, agregar su estado activo y corregir el
  valor predeterminado de dicho estado.
- Índice único para el correo electrónico de los usuarios.
- Documentación interactiva con Scalar y contrato OpenAPI, habilitables por configuración.

### Cambiado

- Los controladores normalizan con `Trim()` los valores de texto externos antes de consultar o
  persistir información.
- Los parámetros de ruta usados como identificadores deben ser enteros positivos.
- Las operaciones de inventario y ventas ahora aplican autorización por rol desde la API.
- Los modelos y DTOs inicializan correctamente sus propiedades no anulables y colecciones.
- La configuración de CORS acepta una lista de orígenes permitidos mediante `Cors:AllowedOrigins`.
- El inicio de la aplicación puede aplicar migraciones automáticamente cuando
  `RUN_MIGRATIONS=true`, pensado para entornos desechables y Docker Compose.

### Corregido

- El mensaje interno en inglés generado por el model binder para rutas como `id=abc` fue sustituido
  por un mensaje contractual en español.
- Se definieron reglas de precisión para importes monetarios y relaciones con eliminación
  restringida donde corresponde.
- Se eliminan las advertencias de referencias anulables durante la compilación.
- El estado `Activo` de nuevos usuarios queda configurado con `true` como valor predeterminado
  mediante una migración tipada de EF Core.

### Seguridad

- Las credenciales incorrectas, las cuentas inexistentes y las cuentas deshabilitadas producen la
  misma respuesta de autenticación para reducir la enumeración de usuarios.
- Las contraseñas se almacenan como hashes generados con `PasswordHasher`; nunca se devuelven en los
  DTOs ni en las respuestas HTTP.
- La API impide que un administrador elimine o deshabilite su propia cuenta.
- La API impide eliminar o deshabilitar al último administrador activo.
- Los recursos de usuarios requieren el rol `Administrador`.
- El backend permanece como frontera de autorización; los permisos no dependen de que la interfaz
  oculte controles.
- Los errores esperados no exponen stack traces, excepciones internas ni mensajes propios del
  framework.
