# Payments Services - Microservicio de Pagos

Microservicio .NET para la gestión y procesamiento de pagos utilizando Stripe como procesador de pagos. Este servicio forma parte de un sistema de gestión de reservas y maneja el registro de medios de pago, procesamiento de transacciones y seguimiento del historial de pagos.

## 📋 Tabla de Contenidos

- [Características](#características)
- [Arquitectura](#arquitectura)
- [Requisitos Previos](#requisitos-previos)
- [Instalación](#instalación)
- [Configuración](#configuración)
- [Ejecución](#ejecución)
- [API Endpoints](#api-endpoints)
- [Tecnologías](#tecnologías)
- [Estructura del Proyecto](#estructura-del-proyecto)

## ✨ Características

- **Gestión de Medios de Pago**: Registro y consulta de métodos de pago de usuarios
- **Procesamiento de Pagos**: Integración con Stripe para procesar pagos de reservas
- **Historial de Transacciones**: Seguimiento completo del historial de pagos por usuario
- **Clean Architecture**: Implementación basada en arquitectura limpia con separación de responsabilidades
- **CQRS Pattern**: Utiliza MediatR para implementar el patrón Command Query Responsibility Segregation
- **Base de Datos PostgreSQL**: Persistencia de datos con Entity Framework Core
- **Dockerizado**: Soporte completo para contenedores Docker

## 🏗️ Arquitectura

El proyecto sigue los principios de Clean Architecture organizados en las siguientes capas:

```
payments_services/
├── src/
│   ├── payments_services.api/          # Capa de presentación (API REST)
│   ├── payments_services.application/  # Lógica de aplicación (Commands, Queries, DTOs)
│   ├── payments_services.domain/       # Entidades del dominio y lógica de negocio
│   └── payments_services.infrastructure/ # Acceso a datos y servicios externos
```

### Capas:

- **API**: Controladores REST y configuración de la aplicación
- **Application**: Casos de uso, Commands, Queries y DTOs
- **Domain**: Entidades del negocio, interfaces y value objects
- **Infrastructure**: Implementación de repositorios, servicios externos (Stripe), y contexto de base de datos

## 📦 Requisitos Previos

Antes de comenzar, asegúrate de tener instalado:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) o superior
- [PostgreSQL 12+](https://www.postgresql.org/download/)
- [Docker](https://www.docker.com/get-started) (opcional, para ejecución en contenedor)
- [Cuenta de Stripe](https://stripe.com) (para obtener las API keys)

## 🚀 Instalación

1. **Clonar el repositorio**:
```bash
git clone https://github.com/eventmesh-lab/payments_services.git
cd payments_services
```

2. **Restaurar dependencias**:
```bash
dotnet restore
```

3. **Configurar la base de datos**:
   - Crear una base de datos PostgreSQL llamada `payments_service`
   - Las migraciones se aplicarán automáticamente al iniciar la aplicación

## ⚙️ Configuración

### Configuración de Base de Datos

Edita el archivo `src/payments_services.api/appsettings.json` para configurar la conexión a PostgreSQL:

```json
{
  "ConnectionStrings": {
    "ConnectionPostgre": "Host=localhost;Port=5432;Database=payments_service;Username=tu_usuario;Password=tu_password"
  }
}
```

### Configuración de Stripe

Configura tus claves de Stripe en `appsettings.json`:

```json
{
  "Stripe": {
    "SecretKey": "sk_test_tu_clave_secreta",
    "PublicKey": "pk_test_tu_clave_publica"
  }
}
```

⚠️ **Importante**: En producción, utiliza variables de entorno o Azure Key Vault para almacenar las claves de forma segura.

### Variables de Entorno (Recomendado para Producción)

```bash
export ConnectionStrings__ConnectionPostgre="Host=localhost;Port=5432;Database=payments_service;Username=usuario;Password=password"
export Stripe__SecretKey="sk_test_tu_clave_secreta"
export Stripe__PublicKey="pk_test_tu_clave_publica"
```

## 🏃 Ejecución

### Ejecución Local

1. **Compilar el proyecto**:
```bash
dotnet build
```

2. **Ejecutar el servicio**:
```bash
cd src/payments_services.api
dotnet run
```

El servicio estará disponible en:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `http://localhost:5000/swagger`

### Ejecución con Docker

1. **Construir la imagen Docker**:
```bash
docker build -t payments-services:latest .
```

2. **Ejecutar el contenedor**:
```bash
docker run -d -p 7183:7183 \
  -e ConnectionStrings__ConnectionPostgre="Host=host.docker.internal;Port=5432;Database=payments_service;Username=postgres;Password=postgres" \
  -e Stripe__SecretKey="tu_clave_secreta" \
  -e Stripe__PublicKey="tu_clave_publica" \
  --name payments-services \
  payments-services:latest
```

### Aplicar Migraciones Manualmente (Opcional)

Si necesitas aplicar migraciones de forma manual:

```bash
cd src/payments_services.infrastructure
dotnet ef database update --startup-project ../payments_services.api
```

## 📡 API Endpoints

### Medios de Pago

#### Registrar Medio de Pago
```http
POST /api/payments/registroMedioDePago
Content-Type: application/json

{
  "correo": "usuario@ejemplo.com",
  "tipoTarjeta": "credit",
  "numeroTarjeta": "4242424242424242",
  "fechaExpiracion": "12/25",
  "cvv": "123"
}
```

#### Consultar Medio de Pago
```http
POST /api/payments/obtenerMedioDePago
Content-Type: application/json

{
  "correo": "usuario@ejemplo.com",
  "idMedioDePago": "pm_xxx"
}
```

#### Obtener Medios de Pago de Usuario
```http
GET /api/payments/obtenerMediosDePagoUsuario/{correo}
```

### Pagos

#### Realizar Pago de Reserva
```http
POST /api/payments/realizarPagoReserva
Content-Type: application/json

{
  "correo": "usuario@ejemplo.com",
  "idReserva": "123",
  "monto": 100.00,
  "idMedioDePago": "pm_xxx"
}
```

#### Obtener Historial de Pagos de Usuario
```http
GET /api/payments/obtenertHistorialPagosUsuario/{correo}
```

#### Obtener Todo el Historial de Pagos
```http
GET /api/payments/obtenertHistorialPagos
```

### Swagger UI

Para explorar todos los endpoints de forma interactiva, visita:
```
http://localhost:5000/swagger
```

## 🛠️ Tecnologías

- **Framework**: .NET 8.0
- **Lenguaje**: C# 12
- **API**: ASP.NET Core Web API
- **ORM**: Entity Framework Core 8.0
- **Base de Datos**: PostgreSQL (Npgsql)
- **Procesador de Pagos**: Stripe.NET
- **Patrones**: 
  - CQRS (MediatR)
  - Repository Pattern
  - Dependency Injection
  - Clean Architecture
- **Documentación**: Swagger/OpenAPI
- **Contenedores**: Docker

### Paquetes NuGet Principales

- `MediatR` - Implementación del patrón mediador
- `Stripe.net` - SDK de Stripe para .NET
- `Npgsql.EntityFrameworkCore.PostgreSQL` - Proveedor PostgreSQL para EF Core
- `Swashbuckle.AspNetCore` - Generación de documentación Swagger

## 📁 Estructura del Proyecto

```
payments_services/
├── src/
│   ├── payments_services.api/
│   │   ├── Controllers/           # Controladores REST
│   │   ├── Program.cs            # Punto de entrada y configuración
│   │   └── appsettings.json      # Configuración de la aplicación
│   │
│   ├── payments_services.application/
│   │   ├── Commands/             # Comandos CQRS
│   │   │   ├── Commands/
│   │   │   └── Handlers/
│   │   ├── Queries/              # Consultas CQRS
│   │   │   ├── Queries/
│   │   │   └── Handlers/
│   │   ├── DTOs/                 # Data Transfer Objects
│   │   ├── Interfaces/           # Interfaces de servicios
│   │   └── Services/             # Servicios de aplicación
│   │
│   ├── payments_services.domain/
│   │   ├── Entities/             # Entidades del dominio
│   │   ├── Interfaces/           # Interfaces del dominio
│   │   ├── ValueObjects/         # Value Objects
│   │   └── Factory/              # Factories del dominio
│   │
│   └── payments_services.infrastructure/
│       ├── Persistence/          # Contexto y repositorios
│       │   ├── Context/
│       │   └── Repositories/
│       ├── ExternalServices/     # Integraciones externas (Stripe)
│       ├── Services/             # Implementación de servicios
│       ├── Mappers/              # Mapeo de entidades
│       └── Migrations/           # Migraciones de EF Core
│
├── Dockerfile                    # Definición de imagen Docker
├── payments_services.sln         # Solución de Visual Studio
└── README.md                     # Este archivo
```

## 🔒 Seguridad

- **Nunca** incluyas claves API reales en el código fuente
- Utiliza variables de entorno o servicios de gestión de secretos en producción
- Las claves de Stripe en `appsettings.json` son solo para desarrollo local
- Implementa autenticación y autorización antes de desplegar en producción
- Utiliza HTTPS en todos los entornos

## 📝 Notas Adicionales

- Las migraciones de Entity Framework se aplican automáticamente al iniciar la aplicación
- El servicio está configurado con CORS para permitir solicitudes desde `http://localhost:3000`
- El servicio se comunica con un servicio de usuarios en `http://localhost:7181/api/users/`
- El puerto predeterminado de la aplicación es 7183

## 🤝 Contribución

Para contribuir al proyecto:

1. Haz fork del repositorio
2. Crea una rama para tu feature (`git checkout -b feature/NuevaCaracteristica`)
3. Realiza tus cambios y haz commit (`git commit -m 'Agrega nueva característica'`)
4. Haz push a la rama (`git push origin feature/NuevaCaracteristica`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto es parte de EventMesh Lab.

## 📧 Soporte

Para preguntas o problemas, por favor abre un issue en el repositorio de GitHub.

---

**EventMesh Lab** - Microservicio de Pagos
