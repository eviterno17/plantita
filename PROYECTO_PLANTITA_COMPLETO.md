# PLANTITA - Documentación Completa del Proyecto

## Tabla de Contenidos

1. [Información General](#información-general)
2. [Arquitectura del Proyecto](#arquitectura-del-proyecto)
3. [Tecnologías Utilizadas](#tecnologías-utilizadas)
4. [Estructura de Directorios](#estructura-de-directorios)
5. [Módulos del Sistema](#módulos-del-sistema)
6. [Código Fuente Completo](#código-fuente-completo)
7. [Configuración](#configuración)
8. [Base de Datos](#base-de-datos)
9. [API Endpoints](#api-endpoints)

---

## Información General

**Plantita** es una plataforma web para la gestión y monitoreo de plantas usando ASP.NET Core Web API con .NET 8.0. El sistema implementa una arquitectura de **Domain-Driven Design (DDD)** con capas claramente separadas.

### Estadísticas del Proyecto
- **Total de archivos C#:** 140
- **Framework:** .NET 8.0
- **Base de datos:** MySQL
- **Patrón de autenticación:** JWT Bearer + BCrypt

---

## Arquitectura del Proyecto

### Patrón de Arquitectura: DDD + 4 Capas

El proyecto sigue una arquitectura de **4 capas por módulo**:

```
┌─────────────────────────────────────┐
│   Interfaces Layer (Controllers)   │  ← REST API, DTOs
├─────────────────────────────────────┤
│   Application Layer (Services)     │  ← Lógica de orquestación
├─────────────────────────────────────┤
│   Domain Layer (Entities/Logic)    │  ← Entidades, Commands, Queries
├─────────────────────────────────────┤
│   Infrastructure Layer (Repos)     │  ← Persistencia, APIs externas
└─────────────────────────────────────┘
```

### Patrones Implementados
- **Domain-Driven Design (DDD)**
- **CQRS** (Command Query Responsibility Segregation)
- **Repository Pattern**
- **Unit of Work**
- **Dependency Injection**
- **Facade Pattern** (ACL)

---

## Tecnologías Utilizadas

### NuGet Packages

```xml
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="EntityFrameworkCore.CreatedUpdatedDate" Version="8.0.0" />
<PackageReference Include="Humanizer" Version="2.14.1" />
<PackageReference Include="MediatR" Version="12.4.1" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.11" />
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.11" />
<PackageReference Include="Microsoft.AspNetCore.SignalR" Version="1.2.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Relational" Version="8.0.11" />
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="8.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="7.2.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.2.1" />
```

### Tecnologías Clave
- **.NET 8.0** - Framework base
- **Entity Framework Core** - ORM
- **MySQL/Pomelo** - Base de datos
- **JWT** - Autenticación
- **BCrypt** - Hash de contraseñas
- **Swagger/OpenAPI** - Documentación API
- **SignalR** - Comunicación en tiempo real

---

## Estructura de Directorios

```
plantita/
├── plantita/
│   ├── Program.cs                          # Punto de entrada de la aplicación
│   ├── plantita.csproj                     # Archivo de proyecto
│   ├── appsettings.json                    # Configuración de la aplicación
│   │
│   ├── ProjectPlantita/                    # Módulos principales del proyecto
│   │   │
│   │   ├── plantmanagment/                 # 🌱 Gestión de Plantas
│   │   │   ├── domain/
│   │   │   │   ├── model/
│   │   │   │   │   ├── aggregates/
│   │   │   │   │   │   ├── MyPlant.cs
│   │   │   │   │   │   └── PlantID/
│   │   │   │   │   │       └── Plant.cs
│   │   │   │   │   ├── Commands/
│   │   │   │   │   │   ├── CreateMyPlantCommand.cs
│   │   │   │   │   │   ├── DeleteMyPlantCommand.cs
│   │   │   │   │   │   ├── UpdateMyPlantCommand.cs
│   │   │   │   │   │   ├── RegisterPlantCommand.cs
│   │   │   │   │   │   ├── SheduleCareTasksCommand.cs
│   │   │   │   │   │   ├── MarkCareTaskCompletedCommand.cs
│   │   │   │   │   │   └── LogPlantHealthStatusCommand.cs
│   │   │   │   │   ├── Entities/
│   │   │   │   │   │   ├── CareTask.cs
│   │   │   │   │   │   └── PlantHealthLog.cs
│   │   │   │   │   └── Queries/
│   │   │   │   │       ├── GetAllMyPlantQuery.cs
│   │   │   │   │       ├── GetMyPlantByIdQuery.cs
│   │   │   │   │       ├── GetPlantByScientificNameQuery.cs
│   │   │   │   │       ├── GetCareTaskByIdQuery.cs
│   │   │   │   │       ├── GetCareTasksByMyPlantIdQuery.cs
│   │   │   │   │       └── GetHealthLogsByMyPlantIdQuery.cs
│   │   │   │   ├── Repositories/
│   │   │   │   │   ├── IPlantRepository.cs
│   │   │   │   │   ├── IMyPlantRepository.cs
│   │   │   │   │   ├── ICareTaskRepository.cs
│   │   │   │   │   └── IPlantHealthLogRepository.cs
│   │   │   │   └── Services/
│   │   │   │       ├── IPlantCommandService.cs
│   │   │   │       ├── IPlantQueryService.cs
│   │   │   │       ├── IMyPlantCommandService.cs
│   │   │   │       ├── IMyPlantQueryService.cs
│   │   │   │       └── IPlantIdentificationService.cs
│   │   │   ├── Application/
│   │   │   │   └── Internal/
│   │   │   │       ├── CommandServices/
│   │   │   │       │   ├── PlantCommandService.cs
│   │   │   │       │   └── MyPlantCommandService.cs
│   │   │   │       └── QueryServices/
│   │   │   │           ├── PlantQueryService.cs
│   │   │   │           ├── MyPlantQueryService.cs
│   │   │   │           └── PlantIdentificationService.cs
│   │   │   ├── Infraestructure/
│   │   │   │   └── EFC/
│   │   │   │       └── Repositories/
│   │   │   │           ├── PlantRepository.cs
│   │   │   │           ├── MyPlantRepository.cs
│   │   │   │           ├── CareTaskRepository.cs
│   │   │   │           └── PlantHealthLogRepository.cs
│   │   │   └── Interfaces/
│   │   │       ├── Controllers/
│   │   │       │   ├── PlantController.cs
│   │   │       │   └── MyPlantController.cs
│   │   │       ├── Resources/
│   │   │       │   ├── PlantResource.cs
│   │   │       │   ├── SavePlantResource.cs
│   │   │       │   ├── MyPlantResource.cs
│   │   │       │   ├── SaveMyPlantResource.cs
│   │   │       │   ├── IdentifiedPlantResource.cs
│   │   │       │   └── PlantImageUploadResource.cs
│   │   │       └── Transform/
│   │   │           ├── PlantTransform.cs
│   │   │           └── MyPlantTransform.cs
│   │   │
│   │   ├── iotmonitoring/                  # 🌡️ Monitoreo IoT
│   │   │   ├── domain/
│   │   │   │   ├── model/
│   │   │   │   │   ├── aggregates/
│   │   │   │   │   │   └── IoTDevice.cs
│   │   │   │   │   └── Entities/
│   │   │   │   │       ├── Sensor.cs
│   │   │   │   │       ├── SensorConfig.cs
│   │   │   │   │       ├── SensorReading.cs
│   │   │   │   │       └── Alert.cs
│   │   │   │   └── repositories/
│   │   │   │       ├── IIoTDeviceRepository.cs
│   │   │   │       ├── ISensorRepository.cs
│   │   │   │       ├── ISensorConfigRepository.cs
│   │   │   │       └── ISensorReadingRepository.cs
│   │   │   ├── Application/
│   │   │   │   └── Internal/
│   │   │   │       └── Services/
│   │   │   │           ├── IIoTDeviceService.cs
│   │   │   │           ├── IoTDeviceService.cs
│   │   │   │           ├── ISensorService.cs
│   │   │   │           ├── SensorService.cs
│   │   │   │           ├── ISensorConfigService.cs
│   │   │   │           ├── SensorConfigService.cs
│   │   │   │           ├── ISensorReadingService.cs
│   │   │   │           └── SensorReadingService.cs
│   │   │   ├── Infraestructure/
│   │   │   │   └── Persistence/
│   │   │   │       ├── IoTDeviceRepository.cs
│   │   │   │       ├── SensorRepository.cs
│   │   │   │       ├── SensorConfigRepository.cs
│   │   │   │       └── SensorReadingRepository.cs
│   │   │   └── Interfaces/
│   │   │       ├── Controllers/
│   │   │       │   ├── IotDeviceController.cs
│   │   │       │   ├── SensorController.cs
│   │   │       │   ├── SensorConfigController.cs
│   │   │       │   └── SensorReadingController.cs
│   │   │       └── Resources/
│   │   │           ├── IoTDeviceResource.cs
│   │   │           ├── SaveIoTDeviceResource.cs
│   │   │           ├── SensorResource.cs
│   │   │           ├── SaveSensorResource.cs
│   │   │           ├── SensorConfigResource.cs
│   │   │           ├── SaveSensorConfigResource.cs
│   │   │           ├── SensorReadingResource.cs
│   │   │           ├── SaveSensorReadingResource.cs
│   │   │           └── EnvironmentDataRecordDto.cs
│   │   │
│   │   ├── diagnosisandproblems/           # 🔬 Diagnóstico (Parcial)
│   │   │   └── domain/
│   │   │       └── model/
│   │   │           ├── aggregates/
│   │   │           │   └── ProblemPlant.cs
│   │   │           └── Entities/
│   │   │               └── Recommendation.cs
│   │   │
│   │   ├── learningandeducation/           # 📚 Educación (Parcial)
│   │   │   └── domain/
│   │   │       └── model/
│   │   │           └── Entities/
│   │   │               ├── EducationalContent.cs
│   │   │               └── UserContentProgress.cs
│   │   │
│   │   ├── communityandsupport/            # 💬 Foro (Parcial)
│   │   │   └── Domain/
│   │   │       └── model/
│   │   │           ├── aggregates/
│   │   │           │   └── QuestionForum.cs
│   │   │           └── Entities/
│   │   │               └── AnswerForum.cs
│   │   │
│   │   └── notifications/                  # 🔔 Notificaciones (Parcial)
│   │       └── domain/
│   │           └── model/
│   │               └── aggregates/
│   │                   └── Notification.cs
│   │
│   ├── User/                               # 👤 Gestión de Usuarios y Autenticación
│   │   ├── Domain/
│   │   │   ├── Model/
│   │   │   │   ├── Aggregates/
│   │   │   │   │   ├── AuthUser.cs
│   │   │   │   │   └── AuthUserRefreshToken.cs
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── SignUpCommand.cs
│   │   │   │   │   ├── SignInCommand.cs
│   │   │   │   │   ├── RefreshTokenRequest.cs
│   │   │   │   │   └── LogoutRequest.cs
│   │   │   │   └── Queries/
│   │   │   │       ├── GetAuthUserByIdQuery.cs
│   │   │   │       ├── GetAuthUserByEmailQuery.cs
│   │   │   │       └── GetAllAuthUsersQuery.cs
│   │   │   ├── Repositories/
│   │   │   │   ├── IAuthUserRepository.cs
│   │   │   │   └── IAuthUserRefreshTokenRepository.cs
│   │   │   └── Services/
│   │   │       ├── IAuthUserCommandService.cs
│   │   │       └── IAuthUserQueryService.cs
│   │   ├── Application/
│   │   │   └── Internal/
│   │   │       ├── CommandServices/
│   │   │       │   └── AuthUserCommandService.cs
│   │   │       ├── QueryServices/
│   │   │       │   └── AuthUserQueryService.cs
│   │   │       └── OutboundServices/
│   │   │           ├── ITokenService.cs
│   │   │           └── IHashingService.cs
│   │   ├── Infraestructure/
│   │   │   ├── Hashing/
│   │   │   │   └── BCrypt/
│   │   │   │       └── Services/
│   │   │   │           └── HashingService.cs
│   │   │   ├── Persistence/
│   │   │   │   └── EFC/
│   │   │   │       └── Repositories/
│   │   │   │           ├── AuthUserRepository.cs
│   │   │   │           └── AuthUserRefreshTokenRepository.cs
│   │   │   ├── Tokens/
│   │   │   │   └── JWT/
│   │   │   │       ├── Services/
│   │   │   │       │   └── TokenService.cs
│   │   │   │       └── Configurations/
│   │   │   │           └── TokenSettings.cs
│   │   │   └── Pipeline/
│   │   │       └── Middleware/
│   │   │           ├── Attributes/
│   │   │           │   ├── AuthorizeAttribute.cs
│   │   │           │   └── AllowAnonymousAttribute.cs
│   │   │           ├── Components/
│   │   │           │   └── RequestAuthorizationMiddleware.cs
│   │   │           └── Extensions/
│   │   │               └── RequestAuthorizationMiddleware.cs
│   │   └── Interfaces/
│   │       ├── ACL/
│   │       │   ├── IIAMContextFacade.cs
│   │       │   └── Services/
│   │       │       └── IAMContextFacade.cs
│   │       └── REST/
│   │           ├── Controllers/
│   │           │   ├── AuthenticationController.cs
│   │           │   ├── AuthUserController.cs
│   │           │   └── BaseController.cs
│   │           ├── Resources/
│   │           │   ├── SignUpResource.cs
│   │           │   ├── SignInResource.cs
│   │           │   ├── AuthUserResource.cs
│   │           │   ├── AuthenticatedUserResource.cs
│   │           │   ├── ForgotPasswordRequest.cs
│   │           │   ├── ResetPasswordRequest.cs
│   │           │   └── VerifyAccountRequest.cs
│   │           └── Transform/
│   │               ├── SignUpCommandFromResourceAssembler.cs
│   │               ├── SignInCommandFromResourceAssembler.cs
│   │               ├── AuthUserResourceFromEntityAssembler.cs
│   │               └── AuthenticatedUserResourceFromEntityAssembler.cs
│   │
│   └── Shared/                             # 🔧 Infraestructura Compartida
│       ├── Domain/
│       │   └── Repositories/
│       │       ├── IBaseRepository.cs
│       │       └── IUnitOfWork.cs
│       └── Infraestructure/
│           ├── Interfaces/
│           │   └── ASP/
│           │       └── Configuration/
│           │           ├── KebabCaseRouteNamingConvention.cs
│           │           └── Extensions/
│           │               └── StringExtentions.cs
│           └── Persistences/
│               └── EFC/
│                   ├── Configuration/
│                   │   ├── AppDbContext.cs
│                   │   ├── IotDeviceConfiguration.cs
│                   │   └── Extensions/
│                   │       ├── ModelBuilderExtensions.cs
│                   │       ├── ModelStateExtensions.cs
│                   │       └── StringExtensions.cs
│                   └── Repositories/
│                       ├── BaseRepository.cs
│                       └── UnitOfWork.cs
│
├── global.json
└── README.md (este archivo)
```

---

## Módulos del Sistema

### 1. Plant Management (Gestión de Plantas) 🌱

**Estado:** Completamente implementado

**Responsabilidades:**
- Gestión del catálogo de plantas (especies)
- Gestión de plantas personales del usuario (MyPlant)
- Identificación de plantas mediante IA (Plant.ID API)
- Registro de tareas de cuidado
- Historial de salud de las plantas

**Componentes principales:**
- **Aggregates:** `Plant`, `MyPlant`
- **Entities:** `CareTask`, `PlantHealthLog`
- **Commands:** 7 comandos CQRS
- **Queries:** 6 consultas
- **Controllers:** `PlantController`, `MyPlantController`

---

### 2. IoT Monitoring (Monitoreo IoT) 🌡️

**Estado:** Completamente implementado

**Responsabilidades:**
- Gestión de dispositivos IoT
- Configuración de sensores
- Registro de lecturas de sensores
- Sistema de alertas basado en umbrales

**Componentes principales:**
- **Aggregates:** `IoTDevice`
- **Entities:** `Sensor`, `SensorConfig`, `SensorReading`, `Alert`
- **Controllers:** `IotDeviceController`, `SensorController`, `SensorConfigController`, `SensorReadingController`

**Tipos de sensores soportados:**
- Temperatura
- Humedad
- Luminosidad
- Humedad del suelo

---

### 3. User Authentication (Autenticación) 👤

**Estado:** Completamente implementado

**Responsabilidades:**
- Registro de usuarios
- Inicio de sesión con JWT
- Refresh tokens
- Gestión de sesiones con cookies HTTP-only

**Características de seguridad:**
- **Hashing:** BCrypt para contraseñas
- **Tokens:** JWT con expiración de 30 minutos
- **Refresh tokens:** 7 días de validez
- **Cookies:** HttpOnly, Secure, SameSite=None

**Controllers:**
- `AuthenticationController` - Sign up, sign in, refresh token, logout
- `AuthUserController` - Gestión de perfiles de usuario

---

### 4. Módulos Parciales (Solo Domain Layer)

#### Diagnosis & Problems 🔬
- **Aggregates:** `ProblemPlant`
- **Entities:** `Recommendation`

#### Learning & Education 📚
- **Entities:** `EducationalContent`, `UserContentProgress`

#### Community & Forum 💬
- **Aggregates:** `QuestionForum`
- **Entities:** `AnswerForum`

#### Notifications 🔔
- **Aggregates:** `Notification`

---

## Código Fuente Completo

### 1. Punto de Entrada - Program.cs

```csharp
// IoT Monitoring dependencies
using plantita.ProjectPlantita.iotmonitoring.Application.Internal.Services;
using plantita.ProjectPlantita.iotmonitoring.domain.repositories;
using plantita.ProjectPlantita.iotmonitoring.Infraestructure.Persistence;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using plantita.ProjectPlantita.plantmanagment.Application.Internal.CommandServices;
using plantita.ProjectPlantita.plantmanagment.Application.Internal.QueryServices;
using plantita.ProjectPlantita.plantmanagment.domain.Repositories;
using plantita.ProjectPlantita.plantmanagment.domain.Services;
using plantita.ProjectPlantita.plantmanagment.Infraestructure.EFC.Repositories;
using plantita.Shared.Domain.Repositories;
using plantita.Shared.Infraestructure.Interfaces.ASP.Configuration;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;
using plantita.Shared.Infraestructure.Persistences.EFC.Repositories;
using plantita.User.Application.Internal.CommandServices;
using plantita.User.Application.Internal.OutboundServices;
using plantita.User.Application.Internal.QueryServices;
using plantita.User.Domain.Repositories;
using plantita.User.Domain.Services;
using plantita.User.Infraestructure.Hashing.BCrypt.Services;
using plantita.User.Infraestructure.Persistence.EFC.Repositories;
using plantita.User.Infraestructure.Tokens.JWT.Configurations;
using plantita.User.Infraestructure.Tokens.JWT.Services;
using plantita.User.Interfaces.ACL;
using plantita.User.Interfaces.ACL.Services;
using ProjectCalculadoraAMSAC.User.Infraestructure.Persistence.EFC.Repositories;
using Swashbuckle.AspNetCore.SwaggerUI;

Console.WriteLine("🔥 Application starting...");

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("✅ Builder creado");

// JWT Authentication Configuration
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer("Bearer", options =>
    {
        var secret = builder.Configuration["TokenSettings:Secret"];
        if (string.IsNullOrEmpty(secret))
        {
            throw new ArgumentNullException(nameof(secret), "JWT Secret is not configured in appsettings.json.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("AuthToken"))
                {
                    context.Token = context.Request.Cookies["AuthToken"];
                }
                return Task.CompletedTask;
            }
        };
    });

// Database Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
        .EnableDetailedErrors(builder.Environment.IsDevelopment());
});

builder.Services.AddControllers(options =>
{
        options.Conventions.Add(new KebabCaseRouteNamingConvention());
});

// Routing Configuration
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",policy =>
    {
        policy.WithOrigins("http://localhost:8080")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();

// Cookie Configuration
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
});

// Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Plantita API",
        Version = "v1",
        Description = "Plantita Platform API",
        TermsOfService = new Uri("http://localhost:5000/swagger/index.html"),
        Contact = new OpenApiContact
        {
            Name = "LlanterosTech",
            Email = "erickpalomino0723@gmail.com"
        },
        License = new OpenApiLicense
        {
            Name = "Apache 2.0",
            Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html")
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT con el prefijo 'Bearer '",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

// Dependency Injection - Shared
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));

// Dependency Injection - User Module
builder.Services.AddScoped<IAuthUserRepository, AuthUserRepository>();
builder.Services.AddScoped<IAuthUserRefreshTokenRepository, AuthUserRefreshTokenRepository>();
builder.Services.AddScoped<IAuthUserCommandService, AuthUserCommandService>();
builder.Services.AddScoped<IAuthUserQueryService, AuthUserQueryService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<IIamContextFacade, IamContextFacade>();

// Dependency Injection - Plant Management Module
builder.Services.AddScoped<IPlantRepository, PlantRepository>();
builder.Services.AddScoped<IMyPlantRepository, MyPlantRepository>();
builder.Services.AddScoped<ICareTaskRepository, CareTaskRepository>();
builder.Services.AddScoped<IPlantHealthLogRepository, PlantHealthLogRepository>();
builder.Services.AddScoped<IPlantCommandService, PlantCommandService>();
builder.Services.AddScoped<IPlantQueryService, PlantQueryService>();
builder.Services.AddScoped<IMyPlantCommandService, MyPlantCommandService>();
builder.Services.AddScoped<IMyPlantQueryService, MyPlantQueryService>();
builder.Services.AddHttpClient<IPlantIdentificationService, PlantIdentificationService>();

// Dependency Injection - IoT Monitoring Module
builder.Services.AddScoped<IIoTDeviceRepository, IoTDeviceRepository>();
builder.Services.AddScoped<ISensorRepository, SensorRepository>();
builder.Services.AddScoped<ISensorConfigRepository, SensorConfigRepository>();
builder.Services.AddScoped<ISensorReadingRepository, SensorReadingRepository>();
builder.Services.AddScoped<IIoTDeviceService, IoTDeviceService>();
builder.Services.AddScoped<ISensorService, SensorService>();
builder.Services.AddScoped<ISensorConfigService, SensorConfigService>();
builder.Services.AddScoped<ISensorReadingService, SensorReadingService>();

var app = builder.Build();
Console.WriteLine("✅ App construida");

// Log Server Addresses
var serverAddresses = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>();

foreach (var address in serverAddresses.Addresses)
{
    Console.WriteLine($"Listening on {address}");
}

// Ensure Database is Created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

// Configure Middleware Pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SupportedSubmitMethods(new[] { SubmitMethod.Get, SubmitMethod.Post });
    c.ConfigObject.AdditionalItems["withCredentials"] = true;
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();
app.Run();
Console.WriteLine("✅ App corriendo");
```

---

### 2. Domain Models

#### 2.1 User - AuthUser.cs

```csharp
using Newtonsoft.Json;
using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;

namespace plantita.User.Domain.Model.Aggregates;

public class AuthUser(string email, string passwordHash,string name,string lastname,string timeZone,string preferredLanguage,DateTime registeredAt,string role)
{
    public AuthUser(): this(string.Empty, string.Empty,string.Empty,string.Empty,string.Empty,string.Empty,DateTime.UtcNow,"User"){}

    public Guid Id { get; }

    public string Email { get; private set; } = email;

    [JsonIgnore] public string PasswordHash { get; private set; } = passwordHash;
    public string Name { get; private set; } = name;
    public string LastName { get; private set; } = lastname;
    public string TimeZone { get; set; } = timeZone;
    public string PreferredLanguage { get; private set; } = preferredLanguage;

    public DateTime RegisteredAt { get; set; } = registeredAt;
    public string Role { get; private set; } = role;

    public List<AuthUserRefreshToken> RefreshTokens { get; set; } = new();


    public AuthUser updateEmail(string email)
    {
        Email = email;
        return this;
    }

    public AuthUser updatePassword(string password)
    {
        PasswordHash = password;
        return this;
    }

    public void SetPassword(string newPassword)
    {
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
    }

    public List<MyPlant> myPlant { get; set; }
}
```

---

#### 2.2 Plant Management - MyPlant.cs

```csharp
using plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates;
using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using plantita.ProjectPlantita.plantmanagment.domain.model.Entities;
using plantita.User.Domain.Model.Aggregates;

namespace plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;

public class MyPlant
{
    public Guid MyPlantId { get; set; }
    public AuthUser AuthUser { get; set; }
    public Guid UserId { get; set; }
    public Guid PlantId { get; set; }
    public string CustomName { get; set; }
    public DateTime AcquiredAt { get; set; }
    public string Location { get; set; }
    public string Note { get; set; }
    public string PhotoUrl { get; set; }
    public string CurrentStatus { get; set; }

    public List<PlantHealthLog> HealthLogs { get; set; }
    public List<CareTask> CareTasks { get; set; }
    public List<Alert> Alerts { get; set; }
    public List<IoTDevice> IoTDevices { get; set; }
}
```

---

#### 2.3 Plant Management - Plant.cs

```csharp
namespace plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.PlantID
{
    public class Plant
    {
        public Guid PlantId { get; set; }
        public string ScientificName { get; set; }
        public string CommonName { get; set; }
        public string Description { get; set; }
        public string Watering { get; set; }
        public string Sunlight { get; set; }
        public string? WikiUrl { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<MyPlant> MyPlants { get; set; }
    }
}
```

---

#### 2.4 Plant Management - CareTask.cs

```csharp
namespace plantita.ProjectPlantita.plantmanagment.domain.model.Entities;

public class CareTask
{
    public Guid TaskId { get; set; }
    public Guid MyPlantId { get; set; }
    public string TaskType { get; set; }
    public DateTime ScheduledFor { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Status { get; set; }
    public string Notes { get; set; }
}
```

---

#### 2.5 IoT Monitoring - IoTDevice.cs

```csharp
using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;

namespace plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates;

public class IoTDevice
{
    public Guid DeviceId { get; set; }
    public Guid AuthUserId { get; set; }
    public Guid MyPlantId { get; set; }
    public string DeviceName { get; set; }
    public string ConnectionType { get; set; }
    public string Location { get; set; }
    public DateTime ActivatedAt { get; set; }
    public string Status { get; set; }
    public string FirmwareVersion { get; set; }

    public List<Sensor> Sensors { get; set; }
    public MyPlant MyPlant { get; set; }
}
```

---

#### 2.6 IoT Monitoring - Sensor.cs

```csharp
using plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates;

namespace plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;

public class Sensor
{
    public Guid SensorId { get; set; }
    public Guid DeviceId { get; set; }
    public string SensorType { get; set; }
    public string Unit { get; set; }
    public decimal RangeMin { get; set; }
    public decimal RangeMax { get; set; }
    public string Model { get; set; }
    public DateTime InstalledAt { get; set; }
    public bool IsActive { get; set; }

    public SensorConfig Configuration { get; set; }
    public IoTDevice Device { get; set; }
    public List<SensorReading> Readings { get; set; }
}
```

---

#### 2.7 IoT Monitoring - Alert.cs

```csharp
using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;

namespace plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;

public class Alert
{
    public Guid AlertId { get; set; }
    public Guid? SensorId { get; set; }
    public Guid? PlantInstanceId { get; set; }
    public string AlertType { get; set; }
    public string Message { get; set; }
    public AlertLevel? Level { get; set; }
    public DateTime? GeneratedAt { get; set; }
    public bool? Seen { get; set; } = false;

    public Sensor Sensor { get; set; }
    public MyPlant PlantInstance { get; set; }
}

public enum AlertLevel
{
    info,
    advertencia,
    crítica
}
```

---

### 3. Controllers

#### 3.1 AuthenticationController.cs

```csharp
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using plantita.User.Application.Internal.OutboundServices;
using plantita.User.Domain.Model.Commands;
using plantita.User.Domain.Repositories;
using plantita.User.Domain.Services;
using plantita.User.Interfaces.REST.Resources;

namespace plantita.User.Interfaces.REST.Controllers;

[Authorize]
[ApiController]
[Route("plantita/v1/authentication")]
[Produces(MediaTypeNames.Application.Json)]
public class AuthenticationController(
    IAuthUserCommandService userCommandService,
    IAuthUserRefreshTokenRepository authUserRefreshTokenRepository,
    IAuthUserRepository authUserRepository,
    ITokenService tokenService) : ControllerBase
{
    [HttpPost("sign-in")]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn([FromBody] SignInResource signInResource)
    {
        var user = await authUserRepository.FindByEmailAsync(signInResource.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(signInResource.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
        }

        var jwtToken = tokenService.GenerateToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        await tokenService.StoreRefreshToken(user.Id, refreshToken);

        Response.Cookies.Append("AuthToken", jwtToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(30),
        });

        Response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(7),
        });

        return Ok(new { message = "Inicio de sesión exitoso",jwtToken = jwtToken,userId = user.Id });
    }

    [HttpPost("sign-up")]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp([FromBody] SignUpResource signUpResource)
    {
        var existingUser = await authUserRepository.FindByEmailAsync(signUpResource.Email);

        if (existingUser != null)
        {
            return BadRequest(new { message = "El usuario ya existe" });
        }

        var signUpCommand = new SignUpCommand(
            signUpResource.Email,
            signUpResource.Password,
            signUpResource.Name,
            signUpResource.LastName,
            signUpResource.timeZone,
            signUpResource.preferredLanguage,
            DateTime.UtcNow,
            "User"
        );

        await userCommandService.Handle(signUpCommand);

        return Ok(new { message = "Usuario creado exitosamente." });
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["RefreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { message = "No hay refresh token disponible" });
        }

        var storedToken = await authUserRefreshTokenRepository.GetByTokenAsync(refreshToken);
        if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
        {
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }

        var user = await authUserRepository.FindByIdAsync(storedToken.UserId);
        if (user == null)
        {
            return Unauthorized(new { message = "Usuario no encontrado" });
        }

        var newJwtToken = tokenService.GenerateToken(user);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        await tokenService.StoreRefreshToken(user.Id, newRefreshToken);

        Response.Cookies.Append("AuthToken", newJwtToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(30),
        });

        Response.Cookies.Append("RefreshToken", newRefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(7),
        });

        return Ok(new { message = "Token renovado exitosamente" });
    }

    [HttpPost("sign-out")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["RefreshToken"];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            await tokenService.RevokeRefreshToken(refreshToken);
        }

        Response.Cookies.Delete("AuthToken");
        Response.Cookies.Delete("RefreshToken");

        return Ok(new { message = "Logout exitoso" });
    }
}
```

---

#### 3.2 PlantController.cs

```csharp
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using plantita.ProjectPlantita.plantmanagment.domain.Services;
using plantita.ProjectPlantita.plantmanagment.Interfaces.Resources;
using plantita.ProjectPlantita.plantmanagment.Interfaces.Transform;

namespace plantita.ProjectPlantita.plantmanagment.Interfaces.Controllers;

[ApiController]
[Route("plantita/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class PlantController(
    IPlantCommandService plantCommandService,
    IPlantQueryService plantQueryService,
    IPlantIdentificationService plantIdentificationService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var plants = await plantQueryService.GetAllPlantsAsync();
        var resources = plants.Select(PlantTransform.ToResource);
        return Ok(resources);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var plant = await plantQueryService.GetByIdAsync(id);
        return plant is null ? NotFound() : Ok(PlantTransform.ToResource(plant));
    }

    [HttpGet("common/{name}")]
    public async Task<IActionResult> GetByCommonName(string name)
    {
        var plant = await plantQueryService.GetByCommonNameAsync(name);
        return plant is null ? NotFound() : Ok(PlantTransform.ToResource(plant));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SavePlantResource resource)
    {
        var model = PlantTransform.ToModel(resource);
        var created = await plantCommandService.RegisterPlantAsync(model);
        return CreatedAtAction(nameof(GetById), new { id = created.PlantId }, PlantTransform.ToResource(created));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SavePlantResource resource)
    {
        var model = PlantTransform.ToModel(resource);
        var updated = await plantCommandService.UpdatePlantAsync(id, model);
        return Ok(PlantTransform.ToResource(updated));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await plantCommandService.DeletePlantAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("identify-and-save")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> IdentifyAndSavePlant([FromForm] PlantImageUploadResource form)
    {
        if (form.Image == null || form.Image.Length == 0)
            return BadRequest("No se ha proporcionado una imagen válida.");

        var result = await plantCommandService.IdentifyAndRegisterPlantAsync(form.Image);
        return result == null
            ? BadRequest("No se pudo identificar ni guardar la planta.")
            : Ok(PlantTransform.ToResource(result));
    }
}
```

---

#### 3.3 MyPlantController.cs

```csharp
using System.Net.Mime;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using plantita.ProjectPlantita.plantmanagment.domain.Services;
using plantita.ProjectPlantita.plantmanagment.Interfaces.Resources;
using plantita.ProjectPlantita.plantmanagment.Interfaces.Transform;

namespace plantita.ProjectPlantita.plantmanagment.Interfaces.Controllers;

[Authorize]
[ApiController]
[Route("plantita/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class MyPlantController : ControllerBase
{
    private readonly IMyPlantCommandService _myPlantCommandService;
    private readonly IPlantQueryService _plantQueryService;
    private readonly IMyPlantQueryService _myPlantQueryService;

    public MyPlantController(IMyPlantCommandService myPlantCommandService,IMyPlantQueryService myPlantQueryService, IPlantQueryService plantQueryService)
    {
        _myPlantCommandService = myPlantCommandService;
        _plantQueryService = plantQueryService;
        _myPlantQueryService = myPlantQueryService;
    }

    [HttpPost("{plantId:guid}")]
    public async Task<IActionResult> Create(Guid plantId, [FromBody] SaveMyPlantResource resource)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized("No se pudo obtener el usuario autenticado.");

        if (!Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized("El identificador de usuario no es válido.");

        var created = await _myPlantCommandService.RegisterMyPlantAsync(userId, plantId, resource);
        return CreatedAtAction(nameof(Create), new { id = created.MyPlantId }, MyPlantTransform.ToResource(created));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        var userId = Guid.Parse(userIdClaim.Value);
        var myPlants = await _myPlantQueryService.GetAllByUserIdAsync(userId);

        var resources = myPlants.Select(MyPlantTransform.ToResource);
        return Ok(resources);
    }

    [HttpGet("{myPlantId:guid}")]
    public async Task<IActionResult> GetMyPlantById(Guid myPlantId)
    {
        var myPlant = await _myPlantQueryService.GetByIdAsync(myPlantId);
        if (myPlant == null) return NotFound();
        var myPlantTransform = MyPlantTransform.ToResource(myPlant);

        return Ok(myPlantTransform);
    }
}
```

---

#### 3.4 IotDeviceController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plantita.ProjectPlantita.iotmonitoring.Application.Internal.Services;
using plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates;
using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using plantita.ProjectPlantita.iotmonitoring.Interfaces.Resources;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;
using System.Security.Claims;

namespace plantita.ProjectPlantita.iotmonitoring.Interfaces.Controllers
{
    [ApiController]
    [Route("plantita/v1/[controller]")]
    public class IotDeviceController : ControllerBase
    {
        private readonly IIoTDeviceService _iotDeviceService;
        private readonly AppDbContext _context;

        public IotDeviceController(IIoTDeviceService iotDeviceService, AppDbContext context)
        {
            _iotDeviceService = iotDeviceService;
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<IoTDeviceResource>> GetAll()
        {
            var devices = await _iotDeviceService.ListAsync();
            return devices.Select(ToResource);
        }

        [HttpGet("me/me")]
        public async Task<IActionResult> GetAllDevicesByUser()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim);
            var myDevices = await _iotDeviceService.GetAllUsersDevicesAsync(userId);

            return Ok(myDevices.Select(ToResource));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<IoTDeviceResource>> GetById(Guid id)
        {
            var device = await _iotDeviceService.GetByIdAsync(id);
            if (device == null) return NotFound();

            return ToResource(device);
        }

        [HttpPost]
        public async Task<ActionResult<IoTDeviceResource>> Create([FromBody] SaveIoTDeviceResource resource)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            var device = new IoTDevice
            {
                AuthUserId = userId,
                MyPlantId = resource.MyPlantId,
                DeviceName = resource.DeviceName,
                ConnectionType = resource.ConnectionType,
                Location = resource.Location,
                ActivatedAt = resource.ActivatedAt,
                Status = resource.Status,
                FirmwareVersion = resource.FirmwareVersion
            };

            var result = await _iotDeviceService.CreateAsync(device);

            return CreatedAtAction(nameof(GetById), new { id = result.DeviceId }, ToResource(result));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<IoTDeviceResource>> Update(Guid id, [FromBody] SaveIoTDeviceResource resource)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            var device = new IoTDevice
            {
                AuthUserId = userId,
                MyPlantId = resource.MyPlantId,
                DeviceName = resource.DeviceName,
                ConnectionType = resource.ConnectionType,
                Location = resource.Location,
                ActivatedAt = resource.ActivatedAt,
                Status = resource.Status,
                FirmwareVersion = resource.FirmwareVersion
            };

            var result = await _iotDeviceService.UpdateAsync(id, device);

            if (result == null) return NotFound();

            return ToResource(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _iotDeviceService.DeleteAsync(id);
            return NoContent();
        }

        private IoTDeviceResource ToResource(IoTDevice entity)
        {
            return new IoTDeviceResource
            {
                DeviceId = entity.DeviceId,
                AuthUserId = entity.AuthUserId,
                MyPlantId = entity.MyPlantId,
                DeviceName = entity.DeviceName,
                ConnectionType = entity.ConnectionType,
                Location = entity.Location,
                ActivatedAt = entity.ActivatedAt,
                Status = entity.Status,
                FirmwareVersion = entity.FirmwareVersion
            };
        }

        // Environment Data Endpoint for IoT Devices
        [HttpPost("/api/v1/environment/data-records")]
        public async Task<IActionResult> CreateEnvironmentRecord([FromBody] EnvironmentDataRecordDto dto)
        {
            var device = await _context.IoTDevices
                .Include(d => d.Sensors)
                .FirstOrDefaultAsync(d => d.DeviceName == dto.CustomDeviceId);

            if (device == null)
                return NotFound("Device not found");

            var readings = new List<SensorReading>();

            if (dto.Light.HasValue)
                AddReadingIfSensorExists(device, "Light", dto.Light.Value, dto.CreatedAt, readings);

            if (dto.SoilMoisture.HasValue)
                AddReadingIfSensorExists(device, "SoilMoisture", dto.SoilMoisture.Value, dto.CreatedAt, readings);

            if (dto.AirTemperature.HasValue)
                AddReadingIfSensorExists(device, "AirTemperature", dto.AirTemperature.Value, dto.CreatedAt, readings);

            if (dto.AirHumidity.HasValue)
                AddReadingIfSensorExists(device, "AirHumidity", dto.AirHumidity.Value, dto.CreatedAt, readings);

            if (!readings.Any())
                return BadRequest("No valid sensor readings found.");

            await _context.SensorReadings.AddRangeAsync(readings);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Data saved", Count = readings.Count });
        }

        private void AddReadingIfSensorExists(IoTDevice device, string sensorType, decimal value, DateTime timestamp, List<SensorReading> readings)
        {
            var sensor = device.Sensors?.FirstOrDefault(s => s.SensorType == sensorType);
            if (sensor != null)
            {
                readings.Add(new SensorReading
                {
                    SensorId = sensor.SensorId,
                    Value = value,
                    Timestamp = timestamp
                });
            }
        }
    }
}
```

---

### 4. Shared Infrastructure - AppDbContext.cs

```csharp
using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Microsoft.EntityFrameworkCore;
using plantita.ProjectPlantita.communityandsupport.Domain.model.Entities;
using plantita.ProjectPlantita.communityandsupport.Domain.model.aggregates;
using plantita.ProjectPlantita.diagnosisandproblems.domain.model.aggregates;
using plantita.ProjectPlantita.diagnosisandproblems.domain.model.Entities;
using plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates;
using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using plantita.ProjectPlantita.learningandeducation.domain.model.Entities;
using plantita.ProjectPlantita.notifications.domain.model.aggregates;
using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;
using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.PlantID;
using plantita.ProjectPlantita.plantmanagment.domain.model.Entities;
using plantita.User.Domain.Model.Aggregates;

namespace plantita.Shared.Infraestructure.Persistences.EFC.Configuration
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
            builder.EnableSensitiveDataLogging();
            builder.AddCreatedUpdatedInterceptor();

            base.OnConfiguring(builder);
        }

        // DbSets
        public DbSet<AuthUser> AuthUsers { get; set; }
        public DbSet<AuthUserRefreshToken> AuthUsersRefreshTokens { get; set; }
        public DbSet<MyPlant> MyPlants { get; set; }
        public DbSet<PlantHealthLog> PlantHealthLogs { get; set; }
        public DbSet<CareTask> CareTasks { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<IoTDevice> IoTDevices { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<SensorConfig> SensorConfigs { get; set; }
        public DbSet<SensorReading> SensorReadings { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<ProblemPlants> ProblemPlants { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<QuestionForum> QuestionForums { get; set; }
        public DbSet<AnswerForum> AnswerForums { get; set; }
        public DbSet<EducationalContent> EducationalContents { get; set; }
        public DbSet<UserContentProgress> UserContentProgresses { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // AuthUser Configuration
            modelBuilder.Entity<AuthUser>(authUser =>
            {
                authUser.HasKey(u => u.Id);
                authUser.Property(u => u.Id).IsRequired();
                authUser.Property(u => u.Email).IsRequired().HasMaxLength(255);
                authUser.HasIndex(u => u.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_AuthUser_Email");
                authUser.Property(u => u.PasswordHash).IsRequired();

                authUser.HasMany(u => u.myPlant)
                    .WithOne(e => e.AuthUser)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // AuthUserRefreshToken Configuration
            modelBuilder.Entity<AuthUserRefreshToken>(refreshToken =>
            {
                refreshToken.HasKey(rt => rt.Id);

                refreshToken.HasOne(rt => rt.AuthUser)
                    .WithMany()
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // MyPlant Configuration
            modelBuilder.Entity<MyPlant>(entity =>
            {
                entity.HasKey(p => p.MyPlantId);
                entity.HasMany(p => p.HealthLogs)
                      .WithOne()
                      .HasForeignKey(h => h.MyPlantId);
                entity.HasMany(p => p.CareTasks)
                      .WithOne()
                      .HasForeignKey(t => t.MyPlantId);
                entity.HasMany(p => p.Alerts)
                      .WithOne(a => a.PlantInstance)
                      .HasForeignKey(a => a.PlantInstanceId);
            });

            // IoTDevice Configuration
            modelBuilder.Entity<IoTDevice>(entity =>
            {
                entity.HasKey(d => d.DeviceId);

                entity.HasOne(d => d.MyPlant)
                    .WithMany(p => p.IoTDevices)
                    .HasForeignKey(d => d.MyPlantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(d => d.Sensors)
                    .WithOne(s => s.Device)
                    .HasForeignKey(s => s.DeviceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Plant Configuration
            modelBuilder.Entity<Plant>(entity =>
            {
                entity.HasKey(p => p.PlantId);
                entity.Property(p => p.ScientificName).IsRequired().HasMaxLength(255);
                entity.Property(p => p.CommonName).HasMaxLength(255);
                entity.Property(p => p.Description).HasMaxLength(2000);
                entity.Property(p => p.Watering).HasMaxLength(100);
                entity.Property(p => p.Sunlight).HasMaxLength(200);
                entity.Property(p => p.WikiUrl).HasMaxLength(500);
                entity.Property(p => p.ImageUrl).HasMaxLength(500);
            });

            // CareTask Configuration
            modelBuilder.Entity<CareTask>(entity =>
            {
                entity.HasKey(ct => ct.TaskId);

                entity.Property(ct => ct.TaskType)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(ct => ct.Status)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(ct => ct.ScheduledFor)
                    .IsRequired();

                entity.HasOne<MyPlant>()
                    .WithMany(p => p.CareTasks)
                    .HasForeignKey(ct => ct.MyPlantId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // PlantHealthLog Configuration
            modelBuilder.Entity<PlantHealthLog>(entity =>
            {
                entity.HasKey(h => h.HealthLogId);

                entity.Property(h => h.HealthStatus)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(h => h.Timestamp)
                    .IsRequired();

                entity.Property(h => h.Source)
                    .HasMaxLength(50);

                entity.HasOne<MyPlant>()
                    .WithMany(p => p.HealthLogs)
                    .HasForeignKey(h => h.MyPlantId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Sensor Configuration
            modelBuilder.Entity<Sensor>(entity =>
            {
                entity.HasKey(s => s.SensorId);
                entity.HasOne(s => s.Configuration)
                      .WithOne()
                      .HasForeignKey<SensorConfig>(c => c.SensorId);
                entity.HasMany(s => s.Readings)
                      .WithOne()
                      .HasForeignKey(r => r.SensorId);
            });

            // SensorReading Configuration
            modelBuilder.Entity<SensorReading>(entity =>
            {
                entity.HasKey(r => r.ReadingId);
                entity.Property(r => r.Value).IsRequired();
            });

            // SensorConfig Configuration
            modelBuilder.Entity<SensorConfig>(entity =>
            {
                entity.HasKey(sc => sc.ConfigId);

                entity.Property(sc => sc.ThresholdMin).IsRequired();
                entity.Property(sc => sc.ThresholdMax).IsRequired();
                entity.Property(sc => sc.FrequencyMinutes).IsRequired();
                entity.Property(sc => sc.AutoNotify).IsRequired();
                entity.Property(sc => sc.ConfiguredAt).IsRequired();

                entity.HasOne<Sensor>()
                    .WithOne(s => s.Configuration)
                    .HasForeignKey<SensorConfig>(sc => sc.SensorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Alert Configuration
            modelBuilder.Entity<Alert>(entity =>
            {
                entity.HasKey(a => a.AlertId);
                entity.HasOne(a => a.Sensor)
                      .WithMany()
                      .HasForeignKey(a => a.SensorId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ProblemPlants Configuration
            modelBuilder.Entity<ProblemPlants>(entity =>
            {
                entity.HasKey(p => p.ProblemId);
                entity.HasMany(p => p.Recommendations)
                      .WithOne()
                      .HasForeignKey(r => r.ProblemId);
            });

            // AnswerForum Configuration
            modelBuilder.Entity<AnswerForum>(entity =>
            {
                entity.HasKey(a => a.AnswerId);

                entity.Property(a => a.Content)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(a => a.AnsweredAt)
                    .IsRequired();

                entity.Property(a => a.IsBestAnswer)
                    .HasDefaultValue(false);

                entity.HasOne<QuestionForum>()
                    .WithMany(q => q.Answers)
                    .HasForeignKey(a => a.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // QuestionForum Configuration
            modelBuilder.Entity<QuestionForum>(entity =>
            {
                entity.HasKey(q => q.QuestionId);
                entity.HasMany(q => q.Answers)
                      .WithOne()
                      .HasForeignKey(a => a.QuestionId);
            });

            // UserContentProgress Configuration
            modelBuilder.Entity<UserContentProgress>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ContentId });
            });

            // EducationalContent Configuration
            modelBuilder.Entity<EducationalContent>(entity =>
            {
                entity.HasKey(ec => ec.ContentId);

                entity.Property(ec => ec.Title).IsRequired().HasMaxLength(255);
                entity.Property(ec => ec.Description).HasMaxLength(1000);
                entity.Property(ec => ec.Type).HasMaxLength(50);
                entity.Property(ec => ec.Level).HasMaxLength(50);
                entity.Property(ec => ec.Url).IsRequired();
                entity.Property(ec => ec.PublishedAt).IsRequired();
            });
        }
    }
}
```

---

## Configuración

### appsettings.json

```json
{
  "TokenSettings": {
    "Secret": "93810e5900bc3d12b9d6aa28bc8fba502e17201435274028ed3327c6593f32b8"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;port=3306;Database=plantita;Username=root;Password=#mimi1234;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "PlantIdApiKey": "wXxf1TF0TYlEmBQESlF7W7Mu8sM49C8Anfo86N5rXz2QiER12s",
  "AllowedHosts": "*"
}
```

### plantita.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
    <PackageReference Include="EntityFrameworkCore.CreatedUpdatedDate" Version="8.0.0" />
    <PackageReference Include="Humanizer" Version="2.14.1" />
    <PackageReference Include="jnm2.ReferenceAssemblies.net35" Version="1.0.1">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="MediatR" Version="12.4.1" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.11" />
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.11" />
    <PackageReference Include="Microsoft.AspNetCore.SignalR" Version="1.2.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.11" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.11">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Relational" Version="8.0.11" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Relational.Design" Version="1.1.6" />
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.0" />
    <PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="8.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="7.2.0" />
    <PackageReference Include="Swashbuckle.AspNetCore.Annotations" Version="7.0.0" />
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.2.1" />
  </ItemGroup>

  <ItemGroup>
    <Folder Include="wwwroot\uploads\" />
  </ItemGroup>

</Project>
```

---

## Base de Datos

### Esquema de Base de Datos (13 Tablas)

#### Tablas de Usuario
1. **AuthUsers** - Usuarios del sistema
2. **AuthUserRefreshTokens** - Tokens de refresco para autenticación

#### Tablas de Gestión de Plantas
3. **Plants** - Catálogo de especies de plantas
4. **MyPlants** - Plantas personales de cada usuario
5. **CareTasks** - Tareas de cuidado programadas
6. **PlantHealthLogs** - Historial de salud de las plantas

#### Tablas de IoT
7. **IoTDevices** - Dispositivos IoT registrados
8. **Sensors** - Sensores asociados a dispositivos
9. **SensorConfigs** - Configuraciones de sensores
10. **SensorReadings** - Lecturas de sensores

#### Tablas de Alertas
11. **Alerts** - Alertas generadas por sensores

#### Tablas de Diagnóstico
12. **ProblemPlants** - Problemas identificados en plantas
13. **Recommendations** - Recomendaciones para problemas

#### Tablas de Comunidad (Parcial)
14. **QuestionForums** - Preguntas del foro
15. **AnswerForums** - Respuestas del foro

#### Tablas de Educación (Parcial)
16. **EducationalContents** - Contenido educativo
17. **UserContentProgresses** - Progreso de usuarios en contenidos

#### Tablas de Notificaciones (Parcial)
18. **Notifications** - Notificaciones del sistema

---

## API Endpoints

### Authentication Endpoints

```
POST   /plantita/v1/authentication/sign-up        - Registro de usuario
POST   /plantita/v1/authentication/sign-in        - Inicio de sesión
POST   /plantita/v1/authentication/refresh-token  - Renovar token
POST   /plantita/v1/authentication/sign-out       - Cerrar sesión
```

### User Endpoints

```
GET    /plantita/v1/auth-user                     - Obtener todos los usuarios
GET    /plantita/v1/auth-user/{id}                - Obtener usuario por ID
GET    /plantita/v1/auth-user/me                  - Obtener usuario autenticado
```

### Plant Management Endpoints

```
GET    /plantita/v1/plant                         - Listar todas las plantas
GET    /plantita/v1/plant/{id}                    - Obtener planta por ID
GET    /plantita/v1/plant/common/{name}           - Buscar planta por nombre común
POST   /plantita/v1/plant                         - Crear nueva planta
POST   /plantita/v1/plant/identify-and-save       - Identificar planta con IA
PUT    /plantita/v1/plant/{id}                    - Actualizar planta
DELETE /plantita/v1/plant/{id}                    - Eliminar planta
```

### My Plant Endpoints (Requiere autenticación)

```
GET    /plantita/v1/my-plant                      - Listar mis plantas
GET    /plantita/v1/my-plant/{id}                 - Obtener mi planta por ID
POST   /plantita/v1/my-plant/{plantId}            - Agregar planta a mi colección
```

### IoT Device Endpoints

```
GET    /plantita/v1/iot-device                    - Listar todos los dispositivos
GET    /plantita/v1/iot-device/me/me              - Listar mis dispositivos
GET    /plantita/v1/iot-device/{id}               - Obtener dispositivo por ID
POST   /plantita/v1/iot-device                    - Crear dispositivo
PUT    /plantita/v1/iot-device/{id}               - Actualizar dispositivo
DELETE /plantita/v1/iot-device/{id}               - Eliminar dispositivo
POST   /api/v1/environment/data-records           - Recibir datos de sensores IoT
```

### Sensor Endpoints

```
GET    /plantita/v1/sensor                        - Listar todos los sensores
GET    /plantita/v1/sensor/{id}                   - Obtener sensor por ID
GET    /plantita/v1/sensor/device/{deviceId}      - Listar sensores por dispositivo
POST   /plantita/v1/sensor                        - Crear sensor
PUT    /plantita/v1/sensor/{id}                   - Actualizar sensor
DELETE /plantita/v1/sensor/{id}                   - Eliminar sensor
```

### Sensor Config Endpoints

```
GET    /plantita/v1/sensor-config                 - Listar configuraciones
GET    /plantita/v1/sensor-config/{id}            - Obtener configuración
POST   /plantita/v1/sensor-config                 - Crear configuración
PUT    /plantita/v1/sensor-config/{id}            - Actualizar configuración
DELETE /plantita/v1/sensor-config/{id}            - Eliminar configuración
```

### Sensor Reading Endpoints

```
GET    /plantita/v1/sensor-reading                - Listar lecturas
GET    /plantita/v1/sensor-reading/{id}           - Obtener lectura
GET    /plantita/v1/sensor-reading/{id}/sensorID  - Lecturas por sensor
POST   /plantita/v1/sensor-reading                - Crear lectura
PUT    /plantita/v1/sensor-reading/{id}           - Actualizar lectura
DELETE /plantita/v1/sensor-reading/{id}           - Eliminar lectura
```

---

## Resumen del Estado del Proyecto

### Módulos Completamente Implementados ✅
- **User Authentication** - Sistema completo de autenticación JWT
- **Plant Management** - Gestión de plantas con identificación IA
- **IoT Monitoring** - Monitoreo completo de dispositivos y sensores

### Módulos Parcialmente Implementados ⚠️
- **Diagnosis & Problems** - Solo capa de dominio
- **Learning & Education** - Solo capa de dominio
- **Community & Forum** - Solo capa de dominio
- **Notifications** - Solo capa de dominio

### Características Destacadas 🌟
- Arquitectura DDD bien estructurada
- Autenticación segura con JWT y BCrypt
- Integración con Plant.ID API para identificación de plantas
- Sistema IoT para monitoreo en tiempo real
- Base de datos MySQL con Entity Framework Core
- Documentación API con Swagger/OpenAPI

---

## Información de Contacto

**Equipo de Desarrollo:** LlanterosTech
**Email:** erickpalomino0723@gmail.com
**Licencia:** Apache 2.0

---

**Fecha de generación:** 2025-11-10
**Versión del framework:** .NET 8.0
**Total de archivos C#:** 140
