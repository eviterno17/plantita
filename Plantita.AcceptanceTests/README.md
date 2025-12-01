# Plantita - BDD Acceptance Tests

Este proyecto contiene pruebas de aceptación BDD (Behavior-Driven Development) completas para la plataforma Plantita, implementadas con **SpecFlow** y **NUnit**.

## 🎯 Objetivo

Validar el comportamiento de la aplicación Plantita desde la perspectiva del usuario utilizando pruebas escritas en lenguaje natural (Gherkin) que son ejecutables.

## 📋 Contenido

### Features (Pruebas Gherkin)

Las pruebas están organizadas por dominio de negocio:

1. **authentication.feature** - Autenticación y autorización de usuarios
2. **plant_catalog.feature** - Gestión del catálogo de plantas
3. **my_plant_collection.feature** - Colección personal de plantas del usuario
4. **iot_device_management.feature** - Gestión de dispositivos IoT
5. **sensor_monitoring.feature** - Monitoreo de sensores y datos ambientales

### Step Definitions (Implementación)

Cada feature tiene su correspondiente archivo de step definitions:

- `AuthenticationSteps.cs` - Pasos para autenticación
- `PlantCatalogSteps.cs` - Pasos para catálogo de plantas
- `MyPlantCollectionSteps.cs` - Pasos para colección personal
- `CommonSteps.cs` - Pasos compartidos entre features

### Infraestructura de Pruebas

- **CustomWebApplicationFactory.cs** - Factory para crear el servidor de pruebas
- **TestContext.cs** - Contexto compartido entre pasos de un escenario
- **ApiClient.cs** - Cliente HTTP para llamadas a la API
- **DatabaseHelper.cs** - Helper para operaciones de base de datos
- **TestHooks.cs** - Hooks de SpecFlow (Before/After)

## 🚀 Requisitos

- .NET 8.0 SDK
- NUnit Test Adapter
- SpecFlow Extension para Visual Studio (opcional, para mejor experiencia)

## 📦 Paquetes Principales

```xml
<PackageReference Include="SpecFlow" Version="3.9.74" />
<PackageReference Include="SpecFlow.NUnit" Version="3.9.74" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.0" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
```

## 🔧 Configuración

### 1. Restaurar paquetes

```bash
dotnet restore
```

### 2. Compilar el proyecto

```bash
dotnet build
```

### 3. Ejecutar las pruebas

```bash
# Ejecutar todas las pruebas
dotnet test

# Ejecutar con verbosidad detallada
dotnet test --logger "console;verbosity=detailed"

# Ejecutar solo pruebas de autenticación
dotnet test --filter "Category=Authentication"

# Ejecutar con cobertura de código
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## 📊 Estructura del Proyecto

```
Plantita.AcceptanceTests/
├── Features/                          # Archivos .feature (Gherkin)
│   ├── authentication.feature
│   ├── plant_catalog.feature
│   ├── my_plant_collection.feature
│   ├── iot_device_management.feature
│   └── sensor_monitoring.feature
│
├── StepDefinitions/                   # Implementación de pasos
│   ├── AuthenticationSteps.cs
│   ├── PlantCatalogSteps.cs
│   ├── MyPlantCollectionSteps.cs
│   └── CommonSteps.cs
│
├── Support/                           # Infraestructura
│   ├── CustomWebApplicationFactory.cs
│   ├── TestContext.cs
│   ├── ApiClient.cs
│   └── DatabaseHelper.cs
│
├── Hooks/                             # Lifecycle hooks
│   └── TestHooks.cs
│
├── specflow.json                      # Configuración de SpecFlow
├── appsettings.Testing.json           # Configuración de pruebas
└── Plantita.AcceptanceTests.csproj    # Proyecto
```

## 🧪 Cómo Funcionan las Pruebas BDD

### 1. Escribir el Feature (Gherkin)

```gherkin
# language: es
Característica: Autenticación de usuarios
  Escenario: Inicio de sesión exitoso
    Dado que existe un usuario registrado con email "[email protected]"
    Cuando envío una solicitud POST a "/plantita/v1/authentication/sign-in" con credenciales válidas
    Entonces debería recibir un código de estado 200
    Y la respuesta debería contener un token JWT
```

### 2. Implementar los Step Definitions

```csharp
[Given(@"que existe un usuario registrado con email ""(.*)""")]
public async Task GivenExisteUnUsuarioRegistradoConEmail(string email)
{
    await _databaseHelper.ExecuteInScope(async db =>
    {
        var user = new AuthUser { Email = email, /* ... */ };
        db.Add(user);
        await db.SaveChangesAsync();
    });
}

[When(@"envío una solicitud POST a ""(.*)"" con credenciales válidas")]
public async Task WhenEnvioUnaSolicitudPOST(string endpoint)
{
    var data = new { email = "[email protected]", password = "Password123!" };
    await _apiClient.PostAsync(endpoint, data);
}

[Then(@"la respuesta debería contener un token JWT")]
public void ThenLaRespuestaDeberiaContenerUnTokenJWT()
{
    var json = JObject.Parse(_testContext.ResponseBody!);
    json["token"].Should().NotBeNullOrEmpty();
}
```

### 3. Ejecutar y Ver Resultados

```bash
dotnet test
```

Resultado:
```
✓ PASSED: Inicio de sesión exitoso
✓ PASSED: Registro de nuevo usuario
✗ FAILED: Login con contraseña incorrecta
```

## 🎨 Características Destacadas

### 1. Base de Datos In-Memory

Las pruebas utilizan Entity Framework In-Memory Database, lo que permite:
- Ejecución rápida
- Aislamiento entre pruebas
- No requiere base de datos real

### 2. Test Server Real

Utiliza `WebApplicationFactory` para crear un servidor de pruebas real:
- Pruebas de integración completas
- Middleware y pipeline completos
- Autenticación JWT real

### 3. Limpieza Automática

Cada escenario comienza con una base de datos limpia:
```csharp
[BeforeScenario]
public void BeforeScenario()
{
    databaseHelper.ResetDatabase().Wait();
}
```

### 4. Contexto Compartido

`TestContext` permite compartir datos entre pasos:
```csharp
_testContext.AccessToken = token;  // En un paso
var token = _testContext.AccessToken; // En otro paso
```

## 📈 Cobertura de Pruebas

### Módulos Cubiertos

✅ **Autenticación (15+ escenarios)**
- Sign up / Sign in
- Token refresh
- Logout
- Validaciones

✅ **Catálogo de Plantas (13+ escenarios)**
- CRUD completo
- Búsqueda
- Identificación con IA

✅ **Colección Personal (16+ escenarios)**
- Gestión de plantas personales
- Tareas de cuidado
- Historial de salud

✅ **IoT y Sensores (35+ escenarios)**
- Dispositivos IoT
- Sensores
- Lecturas ambientales

**Total: 79+ escenarios de prueba**

## 🔍 Debugging

### Ejecutar un solo escenario

```bash
dotnet test --filter "Name~'Inicio de sesión exitoso'"
```

### Ver logs detallados

Descomentar en `TestHooks.cs`:
```csharp
[BeforeStep]
public void BeforeStep(ScenarioContext scenarioContext)
{
    Console.WriteLine($"  → {scenarioContext.StepContext.StepInfo.Text}");
}
```

### Depurar en Visual Studio

1. Poner un breakpoint en el step definition
2. Click derecho en el feature → Debug SpecFlow Scenarios
3. El debugger se detendrá en tu breakpoint

## 📝 Convenciones

### Nombres de Escenarios

✅ Descriptivos y claros:
```gherkin
Escenario: Registro exitoso de un nuevo usuario
```

❌ Evitar nombres genéricos:
```gherkin
Escenario: Test 1
```

### Paso único por acción

✅ Un paso, una acción:
```gherkin
Cuando envío una solicitud POST a "/api/users"
Y la solicitud incluye email válido
```

❌ Evitar pasos compuestos complejos:
```gherkin
Cuando creo un usuario y luego lo actualizo y después lo elimino
```

### Datos de Prueba Realistas

✅ Usar datos realistas:
```gherkin
Dado que existe un usuario con email "[email protected]"
```

❌ Evitar datos poco realistas:
```gherkin
Dado que existe un usuario con email "test"
```

## 🚦 CI/CD Integration

### GitHub Actions

```yaml
name: BDD Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      - name: Run BDD Tests
        run: dotnet test Plantita.AcceptanceTests
```

### Azure DevOps

```yaml
- task: DotNetCoreCLI@2
  displayName: 'Run BDD Acceptance Tests'
  inputs:
    command: 'test'
    projects: '**/Plantita.AcceptanceTests.csproj'
    arguments: '--configuration Release'
```

## 📚 Recursos

- [SpecFlow Documentation](https://docs.specflow.org/)
- [Gherkin Syntax](https://cucumber.io/docs/gherkin/reference/)
- [FluentAssertions](https://fluentassertions.com/)
- [ASP.NET Core Testing](https://learn.microsoft.com/en-us/aspnet/core/test/)

## 🤝 Contribuir

### Agregar un nuevo escenario

1. Editar el archivo `.feature` correspondiente
2. Agregar el escenario en Gherkin
3. Ejecutar las pruebas (fallarán - Red)
4. Implementar los step definitions faltantes
5. Ejecutar las pruebas (deben pasar - Green)
6. Refactorizar si es necesario

### Agregar un nuevo módulo

1. Crear archivo `.feature` en `Features/`
2. Crear archivo step definition en `StepDefinitions/`
3. Implementar pasos
4. Ejecutar y validar

## 🐛 Troubleshooting

### Error: "Ambiguous step definition"

**Problema**: Múltiples step definitions coinciden con el mismo paso.

**Solución**: Hacer las expresiones regulares más específicas.

### Error: "No matching step definition"

**Problema**: Falta implementar un step definition.

**Solución**: SpecFlow genera el código base que puedes copiar y adaptar.

### Pruebas fallan aleatoriamente

**Problema**: Estado compartido entre pruebas.

**Solución**: Verificar que `ResetDatabase()` se ejecuta en `BeforeScenario`.

## 📊 Reportes

### Living Documentation

Generar documentación HTML de las features:

```bash
# Instalar herramienta
dotnet tool install --global SpecFlow.Plus.LivingDoc.CLI

# Generar reporte
livingdoc test-assembly Plantita.AcceptanceTests.dll -t TestExecution.json
```

### Coverage Report

```bash
dotnet test /p:CollectCoverage=true
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:coverage.opencover.xml -targetdir:coverage-report
```

## ✨ Mejores Prácticas

1. **Independencia**: Cada escenario debe ser independiente
2. **Limpieza**: Siempre limpiar datos entre escenarios
3. **Nomenclatura**: Nombres descriptivos en español para Gherkin
4. **Mantenibilidad**: Reutilizar pasos comunes
5. **Velocidad**: Usar in-memory DB para rapidez
6. **Claridad**: Un escenario, un objetivo

## 📞 Soporte

Para preguntas o problemas:
- Revisar la documentación de SpecFlow
- Consultar los ejemplos en este proyecto
- Crear un issue en el repositorio

---

**Versión**: 1.0
**Última actualización**: Diciembre 2024
**Mantenido por**: Equipo Plantita
