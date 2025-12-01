# Quick Start Guide - Plantita BDD Tests

Guía rápida para ejecutar las pruebas BDD de Plantita en 5 minutos.

## ⚡ Inicio Rápido

### 1. Restaurar y Compilar

```bash
cd Plantita.AcceptanceTests
dotnet restore
dotnet build
```

### 2. Ejecutar Todas las Pruebas

```bash
dotnet test
```

### 3. Ver Resultados

```
Test Run Successful.
Total tests: 79
     Passed: 79
     Failed: 0
```

## 🎯 Comandos Útiles

### Ejecutar con Verbosidad

```bash
dotnet test --logger "console;verbosity=detailed"
```

### Ejecutar Solo Autenticación

```bash
dotnet test --filter "FullyQualifiedName~Authentication"
```

### Ejecutar Solo Plantas

```bash
dotnet test --filter "FullyQualifiedName~Plant"
```

### Ejecutar Solo IoT

```bash
dotnet test --filter "FullyQualifiedName~IoT"
```

## 📊 Generar Reporte HTML

### Opción 1: Living Documentation (Recomendado)

```bash
# Instalar herramienta (solo una vez)
dotnet tool install --global SpecFlow.Plus.LivingDoc.CLI

# Ejecutar pruebas y generar JSON
dotnet test --logger "trx;LogFileName=TestResults.trx"

# Generar HTML
livingdoc test-assembly Plantita.AcceptanceTests.dll -t TestResults.trx

# Abrir index.html en navegador
```

### Opción 2: Reporte de Cobertura

```bash
# Ejecutar con cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Instalar ReportGenerator (solo una vez)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generar reporte HTML
reportgenerator -reports:coverage.opencover.xml -targetdir:coverage-report

# Abrir coverage-report/index.html
```

## 🐛 Debug en Visual Studio

1. Abrir `plantita.sln`
2. Ir a Test Explorer (Test > Test Explorer)
3. Click derecho en una prueba → Debug
4. Poner breakpoints en step definitions

## 🐛 Debug en VS Code

1. Abrir carpeta del proyecto
2. Ir a "Run and Debug" (Ctrl+Shift+D)
3. Seleccionar ".NET Core Launch (test)"
4. F5 para iniciar debugging

## 📝 Escribir Nueva Prueba

### 1. Agregar Escenario al Feature

```gherkin
# Features/authentication.feature

Escenario: Nueva prueba
  Dado que estoy en la aplicación
  Cuando hago algo
  Entonces debería ver un resultado
```

### 2. Ejecutar (Fallará)

```bash
dotnet test
```

### 3. SpecFlow Generará el Código Base

```
No matching step definition found for one or more steps.
-> Dado que estoy en la aplicación

[Given(@"que estoy en la aplicación")]
public void GivenQueEstoyEnLaAplicacion()
{
    ScenarioContext.StepIsPending();
}
```

### 4. Copiar e Implementar en StepDefinitions

```csharp
[Given(@"que estoy en la aplicación")]
public void GivenQueEstoyEnLaAplicacion()
{
    // Tu implementación aquí
    _testContext.Store("app_ready", true);
}
```

### 5. Ejecutar de Nuevo (Debería Pasar)

```bash
dotnet test
```

## 🔧 Solución de Problemas Rápidos

### Error: "Program does not contain a static 'Main' method"

**Solución**: Usar `WebApplicationFactory` - ya está configurado en `CustomWebApplicationFactory.cs`

### Error: "Database is locked"

**Solución**: Las pruebas usan InMemory DB, no SQLite. Verificar `CustomWebApplicationFactory.cs`

### Error: "Ambiguous step definition"

**Solución**: Dos step definitions coinciden. Hacer la regex más específica.

### Pruebas Pasan Individualmente pero Fallan Juntas

**Solución**: Verificar que `DatabaseHelper.ResetDatabase()` se ejecuta en `BeforeScenario`

## 📚 Archivos Importantes

| Archivo | Propósito |
|---------|-----------|
| `Features/*.feature` | Escenarios Gherkin |
| `StepDefinitions/*.cs` | Implementación de pasos |
| `Support/CustomWebApplicationFactory.cs` | Test server |
| `Support/TestContext.cs` | Estado compartido |
| `Support/ApiClient.cs` | Cliente HTTP |
| `Support/DatabaseHelper.cs` | Operaciones DB |
| `Hooks/TestHooks.cs` | Before/After scenarios |

## 🎨 Ejemplos de Pruebas

### Ejecutar Escenario Específico

```bash
dotnet test --filter "Name~'Inicio de sesión exitoso'"
```

### Ver Solo Pruebas Fallidas

```bash
dotnet test --logger "console;verbosity=normal" | grep "Failed"
```

### Ejecutar con Timeout Corto

```bash
dotnet test --blame-hang-timeout 30s
```

## 🚀 CI/CD

### GitHub Actions

```yaml
- name: Run BDD Tests
  run: |
    cd Plantita.AcceptanceTests
    dotnet test --no-build --logger trx
```

### GitLab CI

```yaml
test:
  script:
    - cd Plantita.AcceptanceTests
    - dotnet test --logger "junit;LogFileName=test-results.xml"
  artifacts:
    reports:
      junit: test-results.xml
```

## 📞 Ayuda

- **Documentación Completa**: Ver `README.md`
- **SpecFlow Docs**: https://docs.specflow.org/
- **Ejemplos**: Revisar `StepDefinitions/*.cs`

## ✨ Tips

1. **Ejecuta frecuentemente**: `dotnet test` es rápido
2. **Un escenario a la vez**: Más fácil de debuggear
3. **Usa datos realistas**: Mejor que "test1", "test2"
4. **Reutiliza pasos**: Busca en CommonSteps primero
5. **Lee los errores**: SpecFlow te dice qué falta

---

¡Listo para empezar! 🎉

```bash
dotnet test
```
