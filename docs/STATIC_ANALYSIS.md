# Plantita - Static Code Analysis & Verification

Este documento describe el sistema de análisis estático de código implementado en Plantita para garantizar la calidad, seguridad y mantenibilidad del código.

## 📋 Tabla de Contenidos

1. [Overview](#overview)
2. [Herramientas Configuradas](#herramientas-configuradas)
3. [Configuración](#configuración)
4. [Ejecución](#ejecución)
5. [Reglas y Políticas](#reglas-y-políticas)
6. [Integración CI/CD](#integración-cicd)
7. [Resolución de Issues](#resolución-de-issues)

## 🎯 Overview

El sistema de análisis estático de Plantita utiliza múltiples herramientas complementarias para:

- ✅ Detectar bugs potenciales antes de runtime
- ✅ Enforce coding standards y convenciones
- ✅ Identificar vulnerabilidades de seguridad
- ✅ Mejorar rendimiento y mantenibilidad
- ✅ Garantizar consistencia en el código

## 🛠️ Herramientas Configuradas

### 1. Roslyn Analyzers (.NET Built-in)

**Descripción**: Analizadores de código integrados en el compilador de .NET.

**Categorías**:
- Design rules (CA1xxx)
- Globalization rules
- Performance rules
- Security rules
- Reliability rules
- Usage rules

**Configuración**: `plantita.csproj`
```xml
<EnableNETAnalyzers>true</EnableNETAnalyzers>
<AnalysisLevel>latest</AnalysisLevel>
```

**Ejemplo de reglas**:
- `CA1001`: Types that own disposable fields should be disposable
- `CA2100`: Review SQL queries for security vulnerabilities
- `CA1062`: Validate arguments of public methods

### 2. StyleCop.Analyzers

**Versión**: 1.2.0-beta.556

**Descripción**: Enforce code style conventions.

**Categorías**:
- Documentation rules (SA1xxx)
- Layout rules
- Maintainability rules
- Naming rules
- Ordering rules
- Readability rules

**Configuración**: `stylecop.json`

**Ejemplo de reglas**:
- `SA1633`: File should have header
- `SA1101`: Prefix local calls with this
- `SA1200`: Using directives should be placed correctly

**Reglas deshabilitadas**:
- `SA1101`: this prefix (not required)
- `SA1633`: File headers (not required for all files)

### 3. SonarAnalyzer.CSharp

**Versión**: 9.32.0.97167

**Descripción**: Code quality and security analyzer.

**Categorías**:
- Code Smells
- Bugs
- Vulnerabilities
- Security Hotspots

**Ejemplo de reglas**:
- `S125`: Remove commented out code
- `S1066`: Collapsible "if" statements
- `S2068`: Credentials should not be hard-coded
- `S3776`: Cognitive Complexity of methods

### 4. SecurityCodeScan

**Versión**: 5.6.7

**Descripción**: Security vulnerability scanner especializado.

**Detecta**:
- SQL Injection
- XSS (Cross-Site Scripting)
- CSRF vulnerabilities
- Weak cryptography
- Path traversal
- LDAP injection
- XXE (XML External Entities)

**Ejemplo de detecciones**:
```csharp
// ❌ SQL Injection
var query = $"SELECT * FROM Users WHERE Name = '{input}'";

// ❌ Weak crypto
var hash = MD5.Create().ComputeHash(data);

// ❌ Hard-coded password
var password = "admin123";
```

### 5. Roslynator

**Versión**: 4.12.0

**Descripción**: Collection of 500+ analyzers and refactorings.

**Características**:
- Code fixes
- Refactorings
- Best practices
- Modern C# features

**Ejemplo de análisis**:
- Use pattern matching
- Simplify boolean expressions
- Use auto-property
- Remove redundant code

### 6. Meziantou.Analyzer

**Versión**: 2.0.163

**Descripción**: Best practices and modern C# usage.

**Focus áreas**:
- API design
- Async/await patterns
- Exception handling
- String operations
- Collection usage

**Ejemplo de reglas**:
- Prefer `StringComparison` parameter
- Use `ConfigureAwait(false)`
- Prefer `System.HashCode`

### 7. AsyncFixer

**Versión**: 1.6.0

**Descripción**: Analyzer para problemas de async/await.

**Detecta**:
- Blocking calls in async methods
- Missing ConfigureAwait
- Fire and forget calls
- Unnecessary async
- Deadlock risks

**Ejemplo de detecciones**:
```csharp
// ❌ Blocking async call (deadlock risk)
var result = GetDataAsync().Result;

// ❌ Fire and forget
_ = SendEmailAsync();  // No await

// ❌ Missing ConfigureAwait
await repository.SaveAsync();  // Should use ConfigureAwait(false)
```

## ⚙️ Configuración

### Archivo .editorconfig

Ubicación: `/plantita/.editorconfig`

Define:
- Estilo de código (indentación, espaciado, braces)
- Convenciones de nombres
- Preferencias de expresiones
- Severidad de reglas (warning, error, suggestion)

**Ejemplo**:
```ini
[*.cs]
# var preferences
csharp_style_var_when_type_is_apparent = true:suggestion

# Naming conventions
dotnet_naming_rule.interface_should_be_begins_with_i.severity = warning

# Code quality rules
dotnet_diagnostic.CA1001.severity = warning
```

### Archivo stylecop.json

Ubicación: `/plantita/stylecop.json`

Configura StyleCop:
- Company name
- Documentation requirements
- Ordering preferences
- File naming conventions

### Archivo Plantita.ruleset

Ubicación: `/plantita/Plantita.ruleset`

Ruleset personalizado que:
- Define severidad de cada regla
- Habilita/deshabilita reglas específicas
- Configura comportamiento de analyzers

## 🚀 Ejecución

### Durante el Desarrollo

El análisis se ejecuta automáticamente:
```bash
dotnet build
```

### Análisis Completo con Script

```bash
# Dar permisos de ejecución
chmod +x scripts/run-static-analysis.sh

# Ejecutar análisis
./scripts/run-static-analysis.sh
```

**El script**:
1. Limpia build anterior
2. Restaura paquetes
3. Compila con análisis completo
4. Ejecuta todos los analyzers
5. Genera reporte detallado

### Comandos Útiles

```bash
# Build con warnings como errores
dotnet build /p:TreatWarningsAsErrors=true

# Build con análisis máximo
dotnet build /p:AnalysisLevel=latest

# Ver solo warnings
dotnet build | grep "warning"

# Build sin warnings de StyleCop
dotnet build /p:NoWarn=SA1633

# Análisis de proyecto específico
dotnet build plantita/plantita.csproj
```

### Análisis en IDE

**Visual Studio**:
- Warnings aparecen en Error List
- Quick fixes disponibles (Ctrl+.)
- Code cleanup on save

**Visual Studio Code**:
- OmniSharp provides analysis
- Problems panel shows issues
- Quick fixes con (Ctrl+.)

**JetBrains Rider**:
- Real-time code analysis
- Solution-wide analysis
- Code inspections

## 📊 Reglas y Políticas

### Severidad de Reglas

- **Error**: Build falla, debe corregirse
- **Warning**: Build exitoso, pero debe revisarse
- **Info**: Sugerencia, opcional
- **None**: Regla deshabilitada

### Reglas Críticas (Error)

Ninguna por defecto para facilitar desarrollo, pero recomendadas:

```xml
<Rule Id="CA2100" Action="Error" />  <!-- SQL Injection -->
<Rule Id="CA2153" Action="Error" />  <!-- Security -->
<Rule Id="CA5350" Action="Error" />  <!-- Weak Crypto -->
```

### Reglas de Warning

Mayoría de reglas configuradas como Warning:
- Design rules (CA1xxx)
- Security rules (CA2xxx, CA3xxx, CA5xxx)
- Performance rules
- StyleCop rules (configurables)

### Suprimir Warnings

**Opción 1: Global (editorconfig)**
```ini
dotnet_diagnostic.CA1062.severity = none
```

**Opción 2: Por archivo (#pragma)**
```csharp
#pragma warning disable CA1062
public void Method(string param)
{
    // Code that triggers CA1062
}
#pragma warning restore CA1062
```

**Opción 3: SuppressMessage Attribute**
```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "CA1062:Validate arguments of public methods",
    Justification = "Validated by middleware")]
public void Method(string param)
{
    // ...
}
```

## 🔄 Integración CI/CD

### GitHub Actions

```yaml
name: Static Code Analysis

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  analyze:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'

    - name: Restore dependencies
      run: dotnet restore

    - name: Build with analysis
      run: |
        dotnet build \
          --configuration Release \
          /p:EnforceCodeStyleInBuild=true \
          /p:AnalysisLevel=latest \
          /warnasmessage

    - name: Run Static Analysis Script
      run: |
        chmod +x scripts/run-static-analysis.sh
        ./scripts/run-static-analysis.sh

    - name: Upload Analysis Report
      uses: actions/upload-artifact@v3
      with:
        name: static-analysis-report
        path: static-analysis-report.txt
```

### Azure DevOps

```yaml
trigger:
  branches:
    include:
    - main
    - develop

pool:
  vmImage: 'ubuntu-latest'

steps:
- task: DotNetCoreCLI@2
  displayName: 'Restore packages'
  inputs:
    command: 'restore'

- task: DotNetCoreCLI@2
  displayName: 'Build with analysis'
  inputs:
    command: 'build'
    arguments: |
      --configuration Release
      /p:EnforceCodeStyleInBuild=true
      /p:AnalysisLevel=latest

- script: |
    chmod +x scripts/run-static-analysis.sh
    ./scripts/run-static-analysis.sh
  displayName: 'Run static analysis'

- task: PublishBuildArtifacts@1
  displayName: 'Publish analysis report'
  inputs:
    PathtoPublish: 'static-analysis-report.txt'
    ArtifactName: 'StaticAnalysisReport'
```

## 🔍 Resolución de Issues

### Tipos Comunes de Issues

#### 1. CA1062: Validate arguments

```csharp
// ❌ Problema
public void ProcessUser(User user)
{
    var name = user.Name;  // Puede ser null
}

// ✅ Solución
public void ProcessUser(User user)
{
    if (user == null)
        throw new ArgumentNullException(nameof(user));

    var name = user.Name;
}
```

#### 2. CA2100: SQL Injection

```csharp
// ❌ Problema
var query = $"SELECT * FROM Users WHERE Email = '{email}'";

// ✅ Solución
var query = "SELECT * FROM Users WHERE Email = @email";
cmd.Parameters.AddWithValue("@email", email);
```

#### 3. CA5350: Weak Cryptography

```csharp
// ❌ Problema
using var md5 = MD5.Create();

// ✅ Solución
using var sha256 = SHA256.Create();
// O mejor aún, usar BCrypt para passwords
```

#### 4. SA1633: File Header

```csharp
// ❌ Problema: Archivo sin header

// ✅ Solución: Agregar header o deshabilitar regla
// Deshabilitar en .editorconfig:
dotnet_diagnostic.SA1633.severity = none
```

#### 5. AsyncFixer01: Fire and Forget

```csharp
// ❌ Problema
_ = SendEmailAsync();

// ✅ Solución
await SendEmailAsync();

// O si realmente es fire-and-forget:
_ = Task.Run(async () => await SendEmailAsync());
```

### Workflow de Resolución

1. **Identificar el issue**
   - Leer el mensaje de error
   - Entender la regla violada

2. **Investigar**
   - Revisar documentación de la regla
   - Buscar ejemplos de fix

3. **Decidir acción**
   - ¿Es válido el warning?
   - ¿Debe corregirse el código?
   - ¿Debe suprimirse el warning?

4. **Aplicar fix**
   - Corregir el código
   - O suprimir si es falso positivo

5. **Verificar**
   - Recompilar
   - Verificar que el warning desapareció

## 📈 Métricas y Reportes

### Métricas Clave

- **Warning Count**: Número total de warnings
- **Error Count**: Número total de errores
- **Security Issues**: Vulnerabilidades detectadas
- **Code Smells**: Problemas de mantenibilidad

### Objetivo de Calidad

- ✅ 0 errores en build
- ✅ <10 warnings en main branch
- ✅ 0 vulnerabilidades críticas
- ✅ 0 code smells críticos

### Reportes Generados

1. **build-analysis.log**: Log completo de build
2. **analyzers.log**: Output de analyzers
3. **static-analysis-report.txt**: Resumen ejecutivo

## 📚 Referencias

- [.NET Code Analysis](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/overview)
- [StyleCop Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)
- [SonarAnalyzer Rules](https://rules.sonarsource.com/csharp)
- [Roslynator](https://github.com/JosefPihrt/Roslynator)
- [EditorConfig](https://editorconfig.org/)

## 🤝 Contribuir

Al contribuir código:

1. Ejecutar análisis localmente
2. Corregir todos los errores
3. Intentar corregir warnings
4. Documentar supresiones necesarias
5. Incluir justificación para suppressions

---

**Versión**: 1.0
**Última actualización**: Diciembre 2024
**Mantenido por**: Equipo Plantita
