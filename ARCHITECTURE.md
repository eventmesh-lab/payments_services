# Arquitectura del Microservicio de Pagos

Este documento describe la arquitectura técnica y decisiones de diseño del microservicio de pagos.

## 📐 Visión General de la Arquitectura

El microservicio implementa **Clean Architecture** (Arquitectura Limpia) propuesta por Robert C. Martin, también conocida como Arquitectura Hexagonal o Puertos y Adaptadores.

### Principios Fundamentales

1. **Independencia de Frameworks**: La lógica de negocio no depende de frameworks externos
2. **Testabilidad**: La lógica de negocio es fácilmente testeable sin dependencias externas
3. **Independencia de UI**: La UI puede cambiar sin afectar el resto del sistema
4. **Independencia de Base de Datos**: Se puede cambiar PostgreSQL por otra base de datos sin afectar la lógica
5. **Independencia de Servicios Externos**: Los servicios externos son reemplazables

## 🏛️ Capas de la Arquitectura

```
┌─────────────────────────────────────────────────────────┐
│                  payments_services.api                   │
│              (Controllers, Middleware, DI)               │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│             payments_services.application                │
│         (Commands, Queries, DTOs, Interfaces)            │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│              payments_services.domain                    │
│      (Entities, Value Objects, Domain Logic)             │
└─────────────────────────────────────────────────────────┘
                     ▲
                     │
┌────────────────────┴────────────────────────────────────┐
│           payments_services.infrastructure               │
│    (Repositories, External Services, Persistence)        │
└─────────────────────────────────────────────────────────┘
```

### 1. Domain Layer (Capa de Dominio)

**Responsabilidad**: Contiene la lógica de negocio pura y las reglas del dominio.

**Componentes**:
- **Entities**: Objetos con identidad única
  - `MedioDePago`: Representa un método de pago (tarjeta de crédito/débito)
  - `Reserva`: Representa una reserva que requiere pago
  - `HistorialPagos`: Representa el registro de una transacción de pago

- **Value Objects**: Objetos inmutables sin identidad propia
  - Encapsulan conceptos del dominio con validaciones

- **Interfaces**: Contratos que deben cumplir las implementaciones externas
  - Repositorios
  - Servicios del dominio

- **Factory**: Responsables de la creación de objetos complejos del dominio

**Características**:
- No tiene dependencias externas
- No conoce la infraestructura
- Contiene la verdad del negocio

### 2. Application Layer (Capa de Aplicación)

**Responsabilidad**: Orquesta el flujo de datos entre la capa de dominio y las capas externas.

**Patrones Implementados**:

#### CQRS (Command Query Responsibility Segregation)

Separa las operaciones de lectura (Queries) de las operaciones de escritura (Commands).

**Commands** (Escritura):
- `RegistrarMedioPagoCommand`: Registra un nuevo medio de pago
- `RegistrarPagoCommand`: Procesa un pago de reserva
- `RegistrarPagoSubastaCommand`: Procesa un pago de subasta

**Queries** (Lectura):
- `ConsultarMedioDePagoQuery`: Consulta un medio de pago específico
- `ConsultarMediosDePagoQuery`: Consulta todos los medios de pago de un usuario
- `ConsultarHistorialPagosUsuarioQuery`: Consulta el historial de pagos de un usuario
- `ConsultarPagosQuery`: Consulta todos los pagos del sistema

#### MediatR Pattern

Utiliza el patrón Mediador para:
- Desacoplar emisores de receptores
- Centralizar la lógica de orquestación
- Facilitar el manejo de cross-cutting concerns (logging, validación, etc.)

**Flujo de Ejecución**:
```
Controller → IMediator.Send(Command/Query) → Handler → Domain/Infrastructure → Response
```

**Componentes**:
- **DTOs**: Objetos de transferencia de datos
  - `RegistrarMedioDePagoDTO`
  - `RegistrarPagoDTO`
  - `ConsultarMediosDePagoDTO`
  - `ResultadoDTO`

- **Interfaces**: Contratos de servicios de aplicación
  - `IUsuarioService`: Interacción con el servicio de usuarios
  - `IReservaService`: Interacción con el servicio de reservas
  - `IStripeService`: Interacción con Stripe

- **Services**: Servicios de aplicación que coordinan operaciones complejas

### 3. Infrastructure Layer (Capa de Infraestructura)

**Responsabilidad**: Implementa las interfaces definidas en las capas superiores y maneja detalles técnicos.

**Componentes**:

#### Persistence
- **AppDbContext**: Contexto de Entity Framework Core
- **Repositorios**: Implementación del patrón Repository
  - `HistorialPagosRepositoryPostgres`: Acceso a datos del historial de pagos

#### External Services
- **Stripe Integration**:
  - `StripeService`: Servicio principal de Stripe
  - `StripeCustomerService`: Gestión de clientes en Stripe
  - `StripePaymentMethodService`: Gestión de métodos de pago
  - `StripePaymentIntentService`: Procesamiento de intenciones de pago

- **HTTP Clients**:
  - `UsuarioService`: Comunicación con el microservicio de usuarios
  - `ReservaService`: Comunicación con el microservicio de reservas

#### Migrations
- Migraciones de Entity Framework Core para control de versiones de la base de datos

#### Mappers
- Transformación entre entidades de dominio y modelos de persistencia

### 4. API Layer (Capa de Presentación)

**Responsabilidad**: Expone la funcionalidad a través de endpoints HTTP REST.

**Componentes**:

#### Controllers
- `PaymentsController`: Controlador principal con endpoints RESTful
  - Validación de entrada
  - Enrutamiento de comandos/queries a MediatR
  - Formateo de respuestas HTTP

#### Configuration (Program.cs)
- **Dependency Injection**: Configuración del contenedor IoC
- **Middleware Pipeline**: Configuración de CORS, Swagger, autenticación
- **Database Setup**: Configuración de Entity Framework y migraciones automáticas
- **HTTP Clients**: Configuración de clientes HTTP para servicios externos
- **SignalR**: Configuración para notificaciones en tiempo real

## 🔄 Flujo de Datos

### Ejemplo: Registrar un Pago

```
1. Cliente HTTP → POST /api/payments/realizarPagoReserva
                  ↓
2. PaymentsController.RealizarPago()
                  ↓
3. _mediator.Send(new RegistrarPagoCommand(dto))
                  ↓
4. RegistrarPagoHandler.Handle()
                  ↓
5. - IUsuarioService.ObtenerUsuario()
   - IReservaService.ObtenerReserva()
   - IStripePaymentIntentService.CrearPaymentIntent()
                  ↓
6. Domain Validation & Business Logic
                  ↓
7. IHistorialPagosRepository.Save()
                  ↓
8. AppDbContext.SaveChanges() → PostgreSQL
                  ↓
9. Response → Cliente HTTP
```

## 🔌 Integraciones Externas

### Stripe Payment Gateway

**Flujo de Pago**:
1. Cliente registra método de pago → Stripe Payment Method
2. Sistema crea cliente en Stripe → Stripe Customer
3. Se adjunta el método de pago al cliente
4. Se crea un Payment Intent con el monto
5. Se confirma el pago
6. Se guarda el resultado en el historial

**Objetos de Stripe Utilizados**:
- `Customer`: Representa al usuario en Stripe
- `PaymentMethod`: Método de pago (tarjeta)
- `PaymentIntent`: Intención de pago con monto y detalles

### Microservicios Relacionados

- **Usuario Service** (puerto 7181): Gestión de usuarios y autenticación
- **Reserva Service**: Gestión de reservas que requieren pago

## 💾 Modelo de Datos

### Entidades Principales

#### MedioDePago
```csharp
- Id (Guid)
- UsuarioId (string)
- StripePaymentMethodId (string)
- TipoTarjeta (string)
- UltimosDigitos (string)
- Marca (string)
- FechaRegistro (DateTime)
- Activo (bool)
```

#### HistorialPagos
```csharp
- Id (Guid)
- UsuarioId (string)
- ReservaId (string)
- Monto (decimal)
- Estado (string)
- StripePaymentIntentId (string)
- FechaPago (DateTime)
- Descripcion (string)
```

#### Reserva
```csharp
- Id (string)
- UsuarioId (string)
- Monto (decimal)
- Estado (string)
- FechaCreacion (DateTime)
```

## 🛡️ Patrones de Diseño Implementados

### 1. Repository Pattern
Abstrae la lógica de acceso a datos, permitiendo cambiar la implementación sin afectar la lógica de negocio.

### 2. Dependency Injection
Inversión de control para reducir acoplamiento y facilitar testing.

### 3. CQRS (Command Query Responsibility Segregation)
Separa las operaciones de lectura y escritura para optimizar cada una independientemente.

### 4. Mediator Pattern
Desacopla los componentes que se comunican entre sí mediante un mediador central.

### 5. Factory Pattern
Centraliza la creación de objetos complejos del dominio.

### 6. Adapter Pattern
Los servicios de Stripe actúan como adaptadores para integrar el API externa.

## 🚦 Manejo de Errores

### Estrategia de Errores por Capa

**Domain Layer**:
- Lanza excepciones de dominio específicas
- Valida reglas de negocio

**Application Layer**:
- Captura excepciones de dominio
- Transforma a DTOs de respuesta
- Maneja `ApplicationException` para errores de aplicación

**API Layer**:
- Retorna códigos HTTP apropiados
- Estructura de respuesta consistente con `ResultadoDTO`
- Logging de errores

### Códigos HTTP Utilizados

- `200 OK`: Operación exitosa
- `400 Bad Request`: Error de validación o lógica de negocio
- `500 Internal Server Error`: Error inesperado del servidor

## 🔐 Seguridad

### Consideraciones de Seguridad

1. **API Keys**: Las claves de Stripe deben almacenarse de forma segura
2. **CORS**: Configurado para permitir solo orígenes específicos
3. **HTTPS**: Se debe usar en producción
4. **Validación**: Todos los DTOs deben ser validados
5. **Sanitización**: Los datos de entrada deben ser sanitizados

### Mejoras Recomendadas

- [ ] Implementar autenticación JWT
- [ ] Agregar autorización basada en roles
- [ ] Implementar rate limiting
- [ ] Agregar validación de entrada con FluentValidation
- [ ] Implementar circuit breaker para servicios externos
- [ ] Agregar logging estructurado con Serilog

## 📊 Monitoreo y Observabilidad

### Logs
- Logs de aplicación en consola
- Nivel de log configurable por entorno

### Métricas Sugeridas
- Número de transacciones por minuto
- Tasa de éxito/fallo de pagos
- Tiempo de respuesta de endpoints
- Disponibilidad de Stripe API

### Trazabilidad
- Correlation IDs para rastrear requests
- Logs estructurados con contexto

## 🧪 Testing

### Estrategia de Testing

**Unit Tests**: 
- Handlers de comandos/queries
- Lógica de dominio
- Servicios de aplicación

**Integration Tests**:
- Controllers
- Repositorios con base de datos en memoria
- Integración con Stripe (usando mocks)

**End-to-End Tests**:
- Flujos completos de pago
- Escenarios de usuario

## 🔄 Migraciones de Base de Datos

### Entity Framework Core Migrations

Las migraciones se gestionan mediante EF Core y se aplican automáticamente al inicio de la aplicación.

**Crear una nueva migración**:
```bash
cd src/payments_services.infrastructure
dotnet ef migrations add NombreMigracion --startup-project ../payments_services.api
```

**Aplicar migraciones**:
```bash
dotnet ef database update --startup-project ../payments_services.api
```

**Revertir migración**:
```bash
dotnet ef database update MigracionAnterior --startup-project ../payments_services.api
```

## 📈 Escalabilidad

### Consideraciones

1. **Stateless**: El servicio es sin estado, permitiendo escalado horizontal
2. **Database Connection Pooling**: EF Core maneja el pool de conexiones
3. **Async/Await**: Todo el I/O es asíncrono
4. **HTTP Client Factory**: Reuso eficiente de conexiones HTTP

### Estrategias de Escalado

- Múltiples instancias detrás de un load balancer
- Cache distribuido (Redis) para consultas frecuentes
- Cola de mensajes para procesamiento asíncrono de pagos
- Separación de base de datos de lectura y escritura (CQRS completo)

## 🔮 Evolución Futura

### Mejoras Técnicas Sugeridas

1. **Event Sourcing**: Almacenar todos los eventos de pago
2. **Outbox Pattern**: Garantizar consistencia eventual con otros servicios
3. **Health Checks**: Endpoints de salud para orquestadores
4. **Retry Policies**: Políticas de reintento con Polly
5. **Feature Flags**: Control dinámico de características
6. **API Versioning**: Versionado de API para compatibilidad
7. **GraphQL**: Alternativa flexible a REST
8. **gRPC**: Comunicación eficiente entre microservicios

### Funcionalidades de Negocio

1. **Múltiples procesadores de pago**: Abstracción para soportar PayPal, etc.
2. **Pagos recurrentes**: Suscripciones y cobros automáticos
3. **Reembolsos**: Proceso de devolución de pagos
4. **Pagos parciales**: Permitir pagos en cuotas
5. **Multi-moneda**: Soporte para diferentes divisas
6. **Webhooks de Stripe**: Procesamiento de eventos asíncronos

## 📚 Referencias

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern - Martin Fowler](https://martinfowler.com/bliki/CQRS.html)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [Stripe API Documentation](https://stripe.com/docs/api)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

---

Este documento proporciona una visión técnica completa de la arquitectura del microservicio. Para información sobre cómo usar el servicio, consulta el [README.md](README.md).
