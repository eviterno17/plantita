# Pruebas de Aceptación Gherkin - Plantita

Este directorio contiene las pruebas de aceptación escritas en formato Gherkin para la plataforma Plantita.

## Estructura de Archivos

### 1. `authentication.feature`
Pruebas de aceptación para el módulo de autenticación de usuarios.

**Escenarios cubiertos:**
- Registro de nuevos usuarios (sign-up)
- Inicio de sesión (sign-in)
- Actualización de tokens (refresh token)
- Cierre de sesión (sign-out)
- Validaciones de credenciales
- Manejo de errores de autenticación
- Gestión de tokens JWT y cookies HTTP-only

### 2. `plant_catalog.feature`
Pruebas de aceptación para la gestión del catálogo de plantas.

**Escenarios cubiertos:**
- CRUD de plantas en el catálogo
- Búsqueda de plantas por ID y nombre común
- Identificación de plantas mediante IA (Plant.id API)
- Validación de datos de plantas
- Gestión de diferentes tipos de plantas (suculentas, tropicales, cactus, helechos)

### 3. `my_plant_collection.feature`
Pruebas de aceptación para la colección personal de plantas de cada usuario.

**Escenarios cubiertos:**
- Agregar plantas del catálogo a la colección personal
- Consultar y gestionar plantas propias
- Personalización de plantas (nombres, ubicaciones, notas)
- Gestión de fotos de plantas
- Registro y seguimiento de tareas de cuidado
- Historial de salud de plantas
- Control de acceso (solo ver plantas propias)

### 4. `iot_device_management.feature`
Pruebas de aceptación para la gestión de dispositivos IoT.

**Escenarios cubiertos:**
- Registro de dispositivos IoT (WiFi, Bluetooth, Zigbee, LoRa)
- Consulta y actualización de dispositivos
- Activación/desactivación de dispositivos
- Gestión de firmware
- Control de acceso a dispositivos
- Monitoreo de estado de conexión
- Eliminación en cascada de sensores

### 5. `sensor_monitoring.feature`
Pruebas de aceptación para sensores y monitoreo ambiental.

**Escenarios cubiertos:**
- Registro de diferentes tipos de sensores (temperatura, humedad, luz, humedad del suelo)
- Configuración de rangos y unidades de medida
- Registro de lecturas de sensores
- Ingesta masiva de datos
- Consulta de historial de lecturas
- Cálculo de estadísticas (promedios, mínimos, máximos)
- Generación de reportes
- Configuración de alertas
- Control de acceso a sensores

## Formato Gherkin

Las pruebas están escritas en **español** utilizando la sintaxis Gherkin estándar:

```gherkin
# language: es
Característica: Descripción de la funcionalidad
  Como [rol]
  Quiero [objetivo]
  Para [beneficio]

  Escenario: Descripción del escenario
    Dado que [contexto inicial]
    Cuando [acción]
    Entonces [resultado esperado]
```

## Palabras Clave en Español

- **Característica**: Define una funcionalidad de alto nivel
- **Como/Quiero/Para**: User story format
- **Antecedentes**: Pasos comunes a todos los escenarios
- **Escenario**: Un caso de prueba específico
- **Esquema del escenario**: Escenario parametrizado con ejemplos
- **Dado que**: Precondiciones (Given)
- **Cuando**: Acción del usuario (When)
- **Entonces**: Resultado esperado (Then)
- **Y**: Continuación de cualquier paso anterior (And)
- **Ejemplos**: Tabla de datos para esquemas de escenarios

## Frameworks de Testing Compatibles

Estas pruebas pueden ser ejecutadas con diversos frameworks:

### .NET/C# (Recomendado para este proyecto)
- **SpecFlow**: Framework BDD para .NET
- **xUnit/NUnit/MSTest**: Como runners de pruebas

### Otros Frameworks
- **Cucumber** (Ruby, Java, JavaScript)
- **Behave** (Python)
- **Behat** (PHP)

## Instalación de SpecFlow (Recomendado)

Para ejecutar estas pruebas en el proyecto ASP.NET Core:

```bash
# Crear proyecto de pruebas
dotnet new nunit -n Plantita.AcceptanceTests
cd Plantita.AcceptanceTests

# Instalar paquetes necesarios
dotnet add package SpecFlow
dotnet add package SpecFlow.NUnit
dotnet add package SpecFlow.Tools.MsBuild.Generation
dotnet add package FluentAssertions
dotnet add package Microsoft.AspNetCore.Mvc.Testing

# Agregar referencia al proyecto principal
dotnet add reference ../plantita/plantita.csproj
```

## Estructura de Proyecto de Pruebas (Sugerida)

```
Plantita.AcceptanceTests/
├── Features/                    # Archivos .feature (copiar de este directorio)
│   ├── authentication.feature
│   ├── plant_catalog.feature
│   ├── my_plant_collection.feature
│   ├── iot_device_management.feature
│   └── sensor_monitoring.feature
├── StepDefinitions/             # Implementación de pasos
│   ├── AuthenticationSteps.cs
│   ├── PlantCatalogSteps.cs
│   ├── MyPlantCollectionSteps.cs
│   ├── IotDeviceSteps.cs
│   └── SensorMonitoringSteps.cs
├── Support/                     # Código de soporte
│   ├── TestContext.cs
│   ├── ApiClient.cs
│   └── DatabaseHelper.cs
└── Hooks/                       # Hooks de SpecFlow
    └── TestHooks.cs
```

## Ejemplo de Step Definition

```csharp
[Binding]
public class AuthenticationSteps
{
    private readonly ScenarioContext _scenarioContext;
    private readonly ApiClient _apiClient;

    public AuthenticationSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _apiClient = new ApiClient();
    }

    [When(@"envío una solicitud POST a ""(.*)"" con:")]
    public async Task WhenEnvioSolicitudPOSTCon(string endpoint, Table table)
    {
        var data = table.Rows.ToDictionary(r => r["campo"], r => r["valor"]);
        var response = await _apiClient.PostAsync(endpoint, data);
        _scenarioContext["response"] = response;
    }

    [Then(@"debería recibir un código de estado (.*)")]
    public void ThenDeberiaRecibirCodigoEstado(int statusCode)
    {
        var response = _scenarioContext["response"] as HttpResponseMessage;
        response.StatusCode.Should().Be((HttpStatusCode)statusCode);
    }
}
```

## Ejecución de Pruebas

```bash
# Ejecutar todas las pruebas
dotnet test

# Ejecutar pruebas de un feature específico
dotnet test --filter "Category=Authentication"

# Ejecutar con reporte detallado
dotnet test --logger "console;verbosity=detailed"

# Generar reporte HTML (con plugin LivingDoc)
dotnet livingdoc test-assembly Plantita.AcceptanceTests.dll -t TestExecution.json
```

## Cobertura de Pruebas

Las pruebas de aceptación cubren:

✅ **Autenticación y Autorización**
- 15+ escenarios de autenticación
- Manejo de tokens JWT
- Control de acceso

✅ **Gestión de Plantas**
- 13+ escenarios del catálogo
- 16+ escenarios de colección personal
- Integración con IA

✅ **IoT y Sensores**
- 15+ escenarios de dispositivos
- 20+ escenarios de sensores y lecturas
- Monitoreo en tiempo real

✅ **Validaciones y Errores**
- Validación de datos de entrada
- Manejo de errores HTTP
- Mensajes de error descriptivos

## Beneficios de las Pruebas BDD

1. **Documentación Viva**: Las pruebas sirven como documentación actualizada
2. **Comunicación**: Lenguaje común entre desarrolladores, testers y stakeholders
3. **Cobertura**: Garantiza que las funcionalidades cumplen requisitos de negocio
4. **Regresión**: Detecta bugs cuando se modifican funcionalidades existentes
5. **Diseño**: Ayuda a pensar en casos de uso antes de implementar

## Mejores Prácticas

1. **Un escenario, un objetivo**: Cada escenario debe probar una sola cosa
2. **Escenarios independientes**: No deben depender del orden de ejecución
3. **Datos de prueba**: Usar datos realistas pero no sensibles
4. **Limpieza**: Limpiar datos de prueba después de cada escenario
5. **Nomenclatura clara**: Nombres descriptivos que expliquen el propósito
6. **Evitar detalles técnicos**: Enfocarse en comportamiento de negocio

## Próximos Pasos

1. Crear proyecto de pruebas con SpecFlow
2. Implementar Step Definitions para cada feature
3. Configurar base de datos de pruebas
4. Integrar con pipeline CI/CD
5. Generar reportes de ejecución
6. Agregar más escenarios según surjan nuevos requisitos

## Referencias

- [SpecFlow Documentation](https://docs.specflow.org/)
- [Gherkin Reference](https://cucumber.io/docs/gherkin/reference/)
- [BDD Best Practices](https://cucumber.io/docs/bdd/)
- [ASP.NET Core Testing](https://learn.microsoft.com/en-us/aspnet/core/test/)

## Contribución

Al agregar nuevas funcionalidades a Plantita:

1. Escribir primero los escenarios Gherkin
2. Revisar con el equipo
3. Implementar Step Definitions
4. Ejecutar pruebas (deberían fallar - Red)
5. Implementar la funcionalidad
6. Ejecutar pruebas (deberían pasar - Green)
7. Refactorizar si es necesario

---

**Versión**: 1.0
**Fecha**: Diciembre 2024
**Autor**: Equipo Plantita
