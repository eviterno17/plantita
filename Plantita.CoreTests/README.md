# Plantita - Core System Tests

Este proyecto contiene pruebas unitarias y de integración para los componentes core del sistema Plantita, implementadas con **xUnit**, **Moq** y **FluentAssertions**.

## 🎯 Objetivo

Validar la lógica de negocio, entidades de dominio, servicios y repositorios del sistema Plantita mediante pruebas unitarias e integración.

## 📋 Contenido

### Domain Tests (Pruebas de Dominio)

Pruebas unitarias para las entidades y agregados del dominio:

#### User Domain
- **AuthUserTests.cs** - Pruebas del agregado AuthUser
  - Creación de usuarios
  - Validación de propiedades
  - Gestión de refresh tokens
  - Múltiples tokens por usuario

#### Plant Management Domain
- **PlantTests.cs** - Pruebas del agregado Plant
  - Catálogo de plantas
  - Propiedades y características
  - Requisitos de cuidado

- **MyPlantTests.cs** - Pruebas del agregado MyPlant
  - Colección personal de plantas
  - Tareas de cuidado (CareTask)
  - Historial de salud (PlantHealthLog)
  - Relaciones con usuarios

#### IoT Monitoring Domain
- **IoTDeviceTests.cs** - Pruebas del agregado IoTDevice
  - Dispositivos IoT
  - Tipos de conexión (WiFi, Bluetooth, Zigbee, LoRa)
  - Activación/desactivación
  - Gestión de firmware
  - Colección de sensores

- **SensorTests.cs** - Pruebas de entidad Sensor y SensorReading
  - Tipos de sensores (Temperature, SoilMoisture, Light, AirHumidity)
  - Lecturas de sensores
  - Validación de rangos
  - Estadísticas (promedio, min, max)

### Infrastructure Tests (Pruebas de Infraestructura)

#### User Infrastructure
- **BCryptHashingServiceTests.cs** - Pruebas del servicio de hashing
  - Hashing de contraseñas
  - Verificación de contraseñas
  - Seguridad BCrypt
  - Casos especiales (caracteres especiales, unicode)

#### Repository Integration Tests
- **RepositoryIntegrationTests.cs** - Pruebas de integración con EF Core
  - Operaciones CRUD en base de datos
  - Relaciones entre entidades
  - Consultas complejas
  - Cascadas y eliminaciones
  - Múltiples usuarios y aislamiento de datos

### Test Infrastructure (Infraestructura de Pruebas)

#### Fixtures
- **TestDataBuilder.cs** - Constructor de datos de prueba con Bogus
  - Generación de usuarios realistas
  - Generación de plantas
  - Generación de dispositivos IoT
  - Generación de sensores y lecturas
  - Datos en español

#### Helpers
- **AssertionHelpers.cs** - Helpers para assertions comunes
  - Validación de porcentajes
  - Validación de BCrypt
  - Validación de emails
  - Validación de fechas
  - Validación de rangos de sensores

## 🚀 Requisitos

- .NET 8.0 SDK
- xUnit Test Runner

## 📦 Paquetes Principales

```xml
<PackageReference Include="xUnit" Version="2.6.3" />
<PackageReference Include="Moq" Version="4.20.70" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="AutoFixture" Version="4.18.0" />
<PackageReference Include="Bogus" Version="35.3.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
```

## 🔧 Ejecución

### Ejecutar todas las pruebas

```bash
dotnet test
```

### Ejecutar con verbosidad detallada

```bash
dotnet test --logger "console;verbosity=detailed"
```

### Ejecutar solo pruebas de dominio

```bash
dotnet test --filter "FullyQualifiedName~Domain"
```

### Ejecutar solo pruebas de infraestructura

```bash
dotnet test --filter "FullyQualifiedName~Infrastructure"
```

### Ejecutar solo pruebas de un módulo específico

```bash
# Solo User domain
dotnet test --filter "FullyQualifiedName~User"

# Solo Plant Management
dotnet test --filter "FullyQualifiedName~PlantManagement"

# Solo IoT
dotnet test --filter "FullyQualifiedName~IoTMonitoring"
```

### Ejecutar con cobertura de código

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Generar reporte HTML
reportgenerator -reports:coverage.opencover.xml -targetdir:coverage-report
```

## 📊 Estructura del Proyecto

```
Plantita.CoreTests/
├── Domain/                                # Pruebas de dominio
│   ├── User/
│   │   └── AuthUserTests.cs              # Agregado AuthUser
│   ├── PlantManagement/
│   │   ├── PlantTests.cs                 # Agregado Plant
│   │   └── MyPlantTests.cs               # Agregado MyPlant
│   └── IoTMonitoring/
│       ├── IoTDeviceTests.cs             # Agregado IoTDevice
│       └── SensorTests.cs                # Sensor y SensorReading
│
├── Infrastructure/                        # Pruebas de infraestructura
│   ├── User/
│   │   └── BCryptHashingServiceTests.cs  # Servicio de hashing
│   └── RepositoryIntegrationTests.cs     # Repositorios EF Core
│
├── Fixtures/                              # Datos de prueba
│   └── TestDataBuilder.cs                # Constructor de datos
│
├── Helpers/                               # Utilidades
│   └── AssertionHelpers.cs               # Assertions personalizadas
│
└── Plantita.CoreTests.csproj             # Proyecto
```

## 🧪 Tipos de Pruebas

### 1. Pruebas Unitarias de Dominio

Validan la lógica de negocio sin dependencias externas:

```csharp
[Fact]
public void AuthUser_ShouldBeCreated_WithValidData()
{
    // Arrange
    var email = "[email protected]";
    var password = "hashedPassword123";

    // Act
    var user = new AuthUser
    {
        Email = email,
        Password = password,
        Name = "Test User"
    };

    // Assert
    user.Should().NotBeNull();
    user.Email.Should().Be(email);
}
```

### 2. Pruebas de Servicios de Infraestructura

Validan servicios de infraestructura como hashing:

```csharp
[Fact]
public void HashPassword_ShouldReturnHashedPassword()
{
    // Arrange
    var plainPassword = "MySecurePassword123!";

    // Act
    var hashedPassword = _hashingService.HashPassword(plainPassword);

    // Assert
    hashedPassword.Should().NotBeNullOrEmpty();
    hashedPassword.Should().StartWith("$2a$");
}
```

### 3. Pruebas de Integración con Repositorios

Validan operaciones de base de datos con EF Core InMemory:

```csharp
[Fact]
public async Task AuthUser_CanBeSaved_ToDatabase()
{
    // Arrange
    var user = new AuthUser { /* ... */ };

    // Act
    _context.Set<AuthUser>().Add(user);
    await _context.SaveChangesAsync();

    // Assert
    var savedUser = await _context.Set<AuthUser>()
        .FirstOrDefaultAsync(u => u.Email == "[email protected]");

    savedUser.Should().NotBeNull();
}
```

## 📈 Cobertura de Pruebas

### Módulos Cubiertos

✅ **User Domain**
- AuthUser aggregate (10+ tests)
- BCrypt hashing service (12+ tests)
- Refresh tokens

✅ **Plant Management Domain**
- Plant aggregate (8+ tests)
- MyPlant aggregate (15+ tests)
- CareTask entity
- PlantHealthLog entity

✅ **IoT Monitoring Domain**
- IoTDevice aggregate (12+ tests)
- Sensor entity (15+ tests)
- SensorReading entity
- Estadísticas de sensores

✅ **Repository Integration**
- CRUD operations (10+ tests)
- Relaciones entre entidades
- Múltiples usuarios
- Cascadas

**Total: 82+ pruebas unitarias y de integración**

## 🎨 Características Destacadas

### 1. In-Memory Database

Las pruebas de integración utilizan EF Core InMemory:
- Rápidas y aisladas
- No requieren base de datos real
- Cada prueba tiene su propia base de datos

### 2. Test Data Builder

Generación de datos realistas con Bogus:

```csharp
var builder = new TestDataBuilder();
var user = builder.GenerateAuthUser();
var plant = builder.GeneratePlant();
var device = builder.GenerateIoTDevice(user.Id);
```

### 3. Assertion Helpers

Helpers personalizados para validaciones comunes:

```csharp
hashedPassword.ShouldBeValidBCryptHash();
email.ShouldBeValidEmail();
reading.Value.ShouldBeWithinSensorRange(minRange, maxRange);
timestamp.ShouldBeRecent(withinMinutes: 5);
```

### 4. FluentAssertions

Assertions expresivas y legibles:

```csharp
user.RefreshTokens.Should().HaveCount(5);
plant.ScientificName.Should().Be("Monstera deliciosa");
device.Sensors.Should().Contain(s => s.SensorType == "Temperature");
readings.Should().OnlyHaveUniqueItems();
```

## 🔍 Debugging

### Ejecutar una sola prueba

```bash
dotnet test --filter "FullyQualifiedName~AuthUser_ShouldBeCreated_WithValidData"
```

### Ver resultados detallados

```bash
dotnet test --logger "console;verbosity=detailed"
```

### Depurar en Visual Studio

1. Abrir Test Explorer (Test > Test Explorer)
2. Click derecho en una prueba → Debug
3. Poner breakpoints en el código de prueba

### Depurar en VS Code

1. Abrir "Run and Debug" (Ctrl+Shift+D)
2. Seleccionar ".NET Core Attach"
3. F5 para iniciar debugging

## 📝 Convenciones

### Nomenclatura de Pruebas

```
[MethodName]_Should[ExpectedBehavior]_When[Condition]
```

Ejemplos:
- `AuthUser_ShouldBeCreated_WithValidData`
- `HashPassword_ShouldReturnHashedPassword`
- `Plant_CanBeSaved_ToDatabase`
- `Sensor_CanAddReading`

### Estructura AAA (Arrange-Act-Assert)

```csharp
[Fact]
public void ExampleTest()
{
    // Arrange - Preparar datos
    var input = "test";

    // Act - Ejecutar acción
    var result = Method(input);

    // Assert - Verificar resultado
    result.Should().Be("expected");
}
```

### Uso de Theory para Datos Parametrizados

```csharp
[Theory]
[InlineData("WiFi")]
[InlineData("Bluetooth")]
[InlineData("Zigbee")]
public void IoTDevice_ShouldSupportDifferentConnectionTypes(string connectionType)
{
    // ...
}
```

## 🚦 CI/CD Integration

### GitHub Actions

```yaml
- name: Run Core Tests
  run: |
    cd Plantita.CoreTests
    dotnet test --logger trx --collect:"XPlat Code Coverage"
```

### Azure DevOps

```yaml
- task: DotNetCoreCLI@2
  displayName: 'Run Core Tests'
  inputs:
    command: 'test'
    projects: '**/Plantita.CoreTests.csproj'
    arguments: '--configuration Release --collect:"XPlat Code Coverage"'
```

## 🤝 Contribuir

### Agregar una nueva prueba

1. Identificar el módulo (Domain/Infrastructure)
2. Crear archivo de prueba si no existe
3. Escribir prueba siguiendo convenciones AAA
4. Usar nombres descriptivos
5. Ejecutar y verificar

Ejemplo:

```csharp
[Fact]
public void NewEntity_ShouldBehavior_WhenCondition()
{
    // Arrange
    var entity = new NewEntity { /* ... */ };

    // Act
    var result = entity.DoSomething();

    // Assert
    result.Should().NotBeNull();
}
```

### Agregar un nuevo módulo de pruebas

1. Crear carpeta en Domain/ o Infrastructure/
2. Crear archivo de prueba
3. Agregar using statements necesarios
4. Implementar pruebas
5. Ejecutar `dotnet test`

## 🐛 Troubleshooting

### Error: "Cannot access a disposed object"

**Problema**: Intentar usar DbContext después de Dispose.

**Solución**: Usar `IDisposable` pattern correctamente en tests.

### Error: "Sequence contains no elements"

**Problema**: Consulta LINQ no encuentra resultados.

**Solución**: Verificar que los datos de prueba se crearon correctamente.

### Pruebas fallan aleatoriamente

**Problema**: Estado compartido entre pruebas.

**Solución**: Usar una nueva instancia de DbContext para cada prueba.

## 📊 Reportes

### Cobertura de Código

```bash
# Ejecutar con cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Generar reporte HTML
reportgenerator \
  -reports:coverage.opencover.xml \
  -targetdir:coverage-report \
  -reporttypes:Html

# Abrir reporte
open coverage-report/index.html
```

### Resultados de Pruebas

```bash
# Generar archivo TRX
dotnet test --logger "trx;LogFileName=test-results.trx"

# Ver resultados
cat TestResults/test-results.trx
```

## ✨ Mejores Prácticas

1. **Aislamiento**: Cada prueba debe ser independiente
2. **Nomenclatura Clara**: Nombres descriptivos y consistentes
3. **AAA Pattern**: Arrange, Act, Assert siempre
4. **Un Assert por Concepto**: Probar un comportamiento a la vez
5. **Datos Realistas**: Usar TestDataBuilder para datos reales
6. **Fast Tests**: Pruebas rápidas (< 1 segundo)
7. **Determinísticas**: Mismo resultado siempre
8. **Mantenibles**: Fácil de entender y modificar

## 📚 Recursos

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Moq Documentation](https://github.com/moq/moq4)
- [EF Core Testing](https://learn.microsoft.com/en-us/ef/core/testing/)
- [Bogus Documentation](https://github.com/bchavez/Bogus)

## 📞 Soporte

Para preguntas o problemas:
- Revisar esta documentación
- Consultar los ejemplos en el código
- Verificar logs de pruebas
- Crear un issue en el repositorio

---

**Versión**: 1.0
**Última actualización**: Diciembre 2024
**Mantenido por**: Equipo Plantita
