# FuelTrack API

Sistema completo de gestión de pedidos de combustible desarrollado en .NET 8 con arquitectura modular y autenticación JWT.

## 🚀 Características

- **Arquitectura Modular**: Organizado en bounded contexts separados
- **Autenticación JWT**: Con refresh tokens y roles
- **Autorización por Roles**: Admin, Cliente, Proveedor
- **Base de Datos**: MySQL con Entity Framework Core
- **Validación**: FluentValidation para DTOs
- **Logging**: Serilog con archivos y consola
- **Documentación**: Swagger/OpenAPI integrado
- **Pagos**: Sistema simulado de tarjetas de crédito

## 📋 Requisitos Previos

- .NET 8 SDK
- MySQL Server 8.0+
- Visual Studio 2022 o VS Code

## 🛠️ Instalación y Configuración

### 1. Clonar y Configurar

```bash
git clone <repository-url>
cd FuelTrack.Api
```

### 2. Configurar Base de Datos

Editar `appsettings.json` con tu cadena de conexión MySQL:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FuelTrackDb;User=root;Password=tu_password;Port=3306;"
  }
}
```

### 3. Crear y Migrar Base de Datos

```bash
dotnet ef database update
```

### 4. Ejecutar la Aplicación

```bash
dotnet run
```

La API estará disponible en:
- **HTTP**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger

## 👥 Usuarios de Prueba

El sistema incluye usuarios predeterminados:

| Email | Password | Rol |
|-------|----------|-----|
| admin@fueltrack.com | Admin123! | Admin |
| cliente@fueltrack.com | Cliente123! | Cliente |
| proveedor@fueltrack.com | Proveedor123! | Proveedor |

## 🔐 Autenticación

### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@fueltrack.com",
  "password": "Admin123!"
}
```

### Usar Token en Swagger
1. Hacer login y copiar el `accessToken`
2. En Swagger, hacer clic en "Authorize" 🔒
3. Ingresar: `Bearer {tu_token_aqui}`

## 📚 Módulos y Endpoints

### 🔐 Auth (`/api/auth`)
- `POST /login` - Iniciar sesión
- `POST /register` - Registrar usuario
- `POST /refresh` - Renovar token
- `POST /revoke` - Cerrar sesión

### 👥 Users (`/api/users`)
- `GET /` - Listar usuarios (Admin)
- `GET /profile` - Perfil actual
- `PUT /profile` - Actualizar perfil
- `PATCH /{id}/toggle-status` - Activar/desactivar (Admin)

### 📦 Orders (`/api/orders`)
- `GET /` - Listar pedidos (por rol)
- `POST /` - Crear pedido (Cliente)
- `POST /{id}/assign` - Asignar entrega (Proveedor)
- `PATCH /{id}/status` - Actualizar estado (Proveedor)

### 💳 Payments (`/api/payments`)
- `GET /methods` - Métodos de pago (Cliente)
- `POST /methods` - Registrar tarjeta (Cliente)
- `POST /process` - Procesar pago (Cliente)
- `GET /` - Historial de pagos

### 🚛 Vehicles (`/api/vehicles`)
- `GET /` - Listar vehículos (Admin/Proveedor)
- `GET /available` - Vehículos disponibles (Proveedor)
- `POST /` - Crear vehículo (Admin)
- `PATCH /{id}/location` - Actualizar ubicación (Proveedor)

### 👷 Operators (`/api/operators`)
- `GET /` - Listar operadores (Admin/Proveedor)
- `GET /available` - Operadores disponibles (Proveedor)
- `POST /` - Crear operador (Admin)

### 🔔 Notifications (`/api/notifications`)
- `GET /` - Listar notificaciones
- `POST /` - Crear notificación (Admin/Proveedor)
- `PATCH /{id}/read` - Marcar como leída
- `GET /unread-count` - Contador no leídas

### 📊 Analytics (`/api/analytics`)
- `GET /dashboard` - Estadísticas generales (Admin)
- `GET /users` - Estadísticas de usuarios (Admin)
- `GET /provider` - Estadísticas proveedor (Proveedor)
- `GET /client` - Estadísticas cliente (Cliente)

## 🏗️ Arquitectura

```
src/
├── Auth/                 # Autenticación y autorización
├── Users/               # Gestión de usuarios
├── Orders/              # Pedidos de combustible
├── Payments/            # Procesamiento de pagos
├── Vehicles/            # Gestión de vehículos
├── Operators/           # Gestión de operadores
├── Notifications/       # Sistema de notificaciones
├── Analytics/           # Estadísticas y reportes
└── Shared/              # Modelos y servicios compartidos
    ├── Data/           # DbContext y configuración
    ├── Models/         # Entidades de base de datos
    └── Middleware/     # Middleware personalizado
```

## 🔧 Tecnologías Utilizadas

- **.NET 8**: Framework principal
- **Entity Framework Core**: ORM
- **MySQL**: Base de datos
- **JWT**: Autenticación
- **BCrypt**: Hash de contraseñas
- **FluentValidation**: Validación de DTOs
- **Serilog**: Logging
- **Swagger**: Documentación API

## 📝 Flujo de Trabajo

### Cliente
1. Registrarse/Iniciar sesión
2. Registrar método de pago
3. Crear pedido de combustible
4. Pagar pedido
5. Seguir estado de entrega

### Proveedor
1. Ver pedidos pendientes
2. Asignar vehículo y operador
3. Actualizar estado de entrega
4. Enviar notificaciones al cliente

### Admin
1. Gestionar usuarios
2. Administrar vehículos y operadores
3. Ver estadísticas del sistema
4. Monitorear transacciones

## 🚀 Despliegue

Para producción, actualizar:

1. **appsettings.Production.json** con configuración real
2. **JWT SecretKey** con clave segura
3. **Cadena de conexión** a base de datos de producción
4. **Configurar HTTPS** y certificados SSL

## 📞 Soporte

Para soporte técnico o preguntas sobre la implementación, consultar la documentación en Swagger o revisar los logs en la carpeta `logs/`.

---

**FuelTrack API v1.0** - Sistema profesional de gestión de combustible