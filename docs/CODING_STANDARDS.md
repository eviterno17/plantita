# Plantita - Coding Standards & Code Conventions

Este documento describe los estándares de codificación y convenciones de código que deben seguirse en el proyecto Plantita.

## 📋 Tabla de Contenidos

1. [Herramientas de Análisis Estático](#herramientas-de-análisis-estático)
2. [Convenciones de Nombres](#convenciones-de-nombres)
3. [Formato de Código](#formato-de-código)
4. [Reglas de Diseño](#reglas-de-diseño)
5. [Seguridad](#seguridad)
6. [Rendimiento](#rendimiento)
7. [Mantenibilidad](#mantenibilidad)
8. [Documentación](#documentación)

## 🛠️ Herramientas de Análisis Estático

El proyecto utiliza múltiples analizadores de código para garantizar la calidad:

### Analyzers Configurados

#### 1. **Roslyn Analyzers (Built-in .NET)**
- Análisis de código en tiempo de compilación
- Reglas de diseño, rendimiento y seguridad
- Configurado con `<AnalysisLevel>latest</AnalysisLevel>`

#### 2. **StyleCop.Analyzers**
- Enforce code style consistency
- Naming conventions
- Documentation requirements
- Layout and spacing rules

#### 3. **SonarAnalyzer.CSharp**
- Code quality and maintainability
- Code smells detection
- Complexity analysis
- Security vulnerability detection

#### 4. **SecurityCodeScan**
- Security vulnerability scanner
- SQL injection detection
- XSS detection
- Insecure crypto detection

#### 5. **Roslynator**
- Additional refactorings
- Code fixes
- Analyzers for best practices

#### 6. **Meziantou.Analyzer**
- Best practices enforcement
- Modern C# features usage
- API design rules

#### 7. **AsyncFixer**
- Async/await pattern issues
- Deadlock detection
- Synchronization context issues

## 📐 Convenciones de Nombres

### General

```csharp
// ✅ CORRECTO
public class UserService { }
public interface IUserRepository { }
public enum UserStatus { }

// ❌ INCORRECTO
public class userService { }
public interface UserRepository { }  // Falta 'I' prefix
public enum user_status { }
```

### Clases, Interfaces y Structs

- **Clases**: PascalCase
- **Interfaces**: PascalCase con prefijo `I`
- **Structs**: PascalCase
- **Enums**: PascalCase

```csharp
// ✅ Ejemplos correctos
public class AuthUser { }
public interface IUserRepository { }
public struct Point { }
public enum ConnectionType { WiFi, Bluetooth, Zigbee }
```

### Métodos y Propiedades

- **Métodos**: PascalCase
- **Propiedades**: PascalCase
- **Eventos**: PascalCase

```csharp
// ✅ CORRECTO
public string GetFullName() { }
public string FullName { get; set; }
public event EventHandler UserCreated;

// ❌ INCORRECTO
public string getFullName() { }
public string fullName { get; set; }
```

### Campos y Variables

- **Campos privados**: camelCase con prefijo `_`
- **Campos públicos**: PascalCase
- **Constantes**: PascalCase
- **Variables locales**: camelCase
- **Parámetros**: camelCase

```csharp
// ✅ CORRECTO
public class Example
{
    private readonly string _connectionString;
    public string PublicField;
    public const int MaxRetries = 3;

    public void Method(string userName)
    {
        var fullName = GetFullName(userName);
    }
}

// ❌ INCORRECTO
private string connectionString;  // Falta _
public string publicField;         // Debe ser PascalCase
public const int maxRetries = 3;   // Debe ser PascalCase
```

## 📝 Formato de Código

### Indentación

- **Tipo**: Espacios (no tabs)
- **Tamaño**: 4 espacios
- **Braces**: Nueva línea (Allman style)

```csharp
// ✅ CORRECTO
public class Example
{
    public void Method()
    {
        if (condition)
        {
            DoSomething();
        }
    }
}

// ❌ INCORRECTO (K&R style)
public class Example {
    public void Method() {
        if (condition) {
            DoSomething();
        }
    }
}
```

### Espaciado

```csharp
// ✅ CORRECTO
if (x > 0)
{
    var result = x + y;
    Method(arg1, arg2);
}

// ❌ INCORRECTO
if(x>0)
{
    var result=x+y;
    Method(arg1,arg2);
}
```

### Organización de Usings

- System usings primero
- Usings ordenados alfabéticamente
- No agrupar usings

```csharp
// ✅ CORRECTO
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using plantita.Domain.Model;

// ❌ INCORRECTO
using plantita.Domain.Model;
using System.Linq;
using System;
```

### Longitud de Línea

- **Máximo recomendado**: 120 caracteres
- Dividir líneas largas en múltiples líneas cuando sea necesario

## 🎯 Reglas de Diseño

### 1. Uso de `this`

```csharp
// ✅ CORRECTO (no usar 'this' innecesariamente)
public class Example
{
    private string _name;

    public void SetName(string name)
    {
        _name = name;
    }
}

// ❌ INCORRECTO
public void SetName(string name)
{
    this._name = name;  // 'this' es innecesario
}
```

### 2. Modificadores de Acceso

```csharp
// ✅ CORRECTO (siempre especificar)
public class Example
{
    private readonly string _field;
    public string Property { get; set; }
    internal void Method() { }
}

// ❌ INCORRECTO (falta modificador)
class Example  // Debe ser 'public class Example'
{
    string _field;  // Debe ser 'private readonly string _field'
}
```

### 3. Readonly Fields

```csharp
// ✅ CORRECTO
private readonly IUserRepository _userRepository;

// ❌ INCORRECTO (si el campo no se modifica después del constructor)
private IUserRepository _userRepository;
```

### 4. Expression-bodied Members

```csharp
// ✅ Para propiedades simples
public string FullName => $"{FirstName} {LastName}";

// ✅ Para métodos simples (opcional)
public string GetFullName() => $"{FirstName} {LastName}";

// ❌ Para métodos complejos
public string ComplexMethod() => DoThis() && DoThat() ? Result1() : Result2();  // Difícil de leer
```

### 5. Object Initializers

```csharp
// ✅ CORRECTO
var user = new AuthUser
{
    Email = email,
    Name = name,
    Timezone = timezone
};

// ❌ INCORRECTO (cuando object initializer es más claro)
var user = new AuthUser();
user.Email = email;
user.Name = name;
user.Timezone = timezone;
```

### 6. Null Checking

```csharp
// ✅ CORRECTO (null-coalescing)
var name = user?.Name ?? "Unknown";

// ✅ CORRECTO (null-conditional)
var length = user?.Name?.Length;

// ❌ INCORRECTO (verboso)
string name;
if (user != null && user.Name != null)
{
    name = user.Name;
}
else
{
    name = "Unknown";
}
```

### 7. Pattern Matching

```csharp
// ✅ CORRECTO (pattern matching)
if (obj is AuthUser user)
{
    Console.WriteLine(user.Name);
}

// ❌ INCORRECTO
if (obj is AuthUser)
{
    var user = (AuthUser)obj;
    Console.WriteLine(user.Name);
}
```

## 🔒 Seguridad

### 1. SQL Injection Prevention

```csharp
// ✅ CORRECTO (parameterized queries)
var query = "SELECT * FROM Users WHERE Email = @email";
command.Parameters.AddWithValue("@email", email);

// ❌ INCORRECTO (concatenación directa)
var query = $"SELECT * FROM Users WHERE Email = '{email}'";
```

### 2. Validación de Entrada

```csharp
// ✅ CORRECTO
public void ProcessUserInput(string input)
{
    if (string.IsNullOrWhiteSpace(input))
    {
        throw new ArgumentException("Input cannot be null or empty", nameof(input));
    }

    // Sanitize and validate
    var sanitized = SanitizeInput(input);
    // Process...
}
```

### 3. Hashing de Contraseñas

```csharp
// ✅ CORRECTO (BCrypt)
var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

// ❌ INCORRECTO (MD5, SHA1 sin salt)
var hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
```

### 4. Secrets Management

```csharp
// ✅ CORRECTO (Configuration/User Secrets)
var connectionString = _configuration.GetConnectionString("DefaultConnection");
var jwtSecret = _configuration["JwtSettings:Secret"];

// ❌ INCORRECTO (hardcoded)
var connectionString = "Server=localhost;Database=plantita;User=root;Password=admin123";
var jwtSecret = "my-super-secret-key";
```

## ⚡ Rendimiento

### 1. String Concatenation

```csharp
// ✅ CORRECTO (StringBuilder para múltiples concatenaciones)
var sb = new StringBuilder();
foreach (var item in items)
{
    sb.Append(item);
    sb.Append(", ");
}

// ❌ INCORRECTO (+ en loop)
string result = "";
foreach (var item in items)
{
    result += item + ", ";
}
```

### 2. LINQ

```csharp
// ✅ CORRECTO (.Any() para existencia)
if (users.Any())
{
    // ...
}

// ❌ INCORRECTO (.Count() > 0)
if (users.Count() > 0)  // Itera toda la colección
{
    // ...
}
```

### 3. Async/Await

```csharp
// ✅ CORRECTO
public async Task<User> GetUserAsync(int id)
{
    return await _repository.GetByIdAsync(id);
}

// ❌ INCORRECTO (sync over async)
public User GetUser(int id)
{
    return _repository.GetByIdAsync(id).Result;  // Puede causar deadlock
}
```

### 4. Dispose Pattern

```csharp
// ✅ CORRECTO (using statement)
using (var connection = new SqlConnection(connectionString))
{
    // Use connection
}

// ✅ CORRECTO (using declaration)
using var stream = File.OpenRead(path);
// Use stream

// ❌ INCORRECTO (sin dispose)
var connection = new SqlConnection(connectionString);
// Use connection
// Leak!
```

## 🔧 Mantenibilidad

### 1. Complejidad Ciclomática

- **Máximo recomendado**: 10
- Dividir métodos complejos en métodos más pequeños

```csharp
// ✅ CORRECTO (baja complejidad)
public bool IsValidUser(User user)
{
    return IsEmailValid(user.Email)
        && IsPasswordStrong(user.Password)
        && IsNameValid(user.Name);
}

// ❌ INCORRECTO (alta complejidad)
public bool IsValidUser(User user)
{
    if (user == null) return false;
    if (string.IsNullOrEmpty(user.Email)) return false;
    if (!user.Email.Contains("@")) return false;
    if (string.IsNullOrEmpty(user.Password)) return false;
    if (user.Password.Length < 8) return false;
    // ... 10 más condiciones
}
```

### 2. Métodos Pequeños

- **Máximo recomendado**: 50 líneas
- **Ideal**: 10-20 líneas

### 3. Evitar Magic Numbers

```csharp
// ✅ CORRECTO
private const int MaxRetries = 3;
private const int TimeoutSeconds = 30;

if (retryCount < MaxRetries)
{
    // ...
}

// ❌ INCORRECTO
if (retryCount < 3)  // ¿Qué significa 3?
{
    // ...
}
```

### 4. Single Responsibility Principle

```csharp
// ✅ CORRECTO (responsabilidad única)
public class UserRepository
{
    public async Task<User> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }
}

// ❌ INCORRECTO (múltiples responsabilidades)
public class UserService
{
    public async Task<User> GetUser(int id)
    {
        // Database access
        // Email sending
        // Logging
        // Authentication
        // etc.
    }
}
```

## 📚 Documentación

### 1. XML Documentation

```csharp
/// <summary>
/// Retrieves a user by their unique identifier.
/// </summary>
/// <param name="id">The unique identifier of the user.</param>
/// <returns>The user if found; otherwise, null.</returns>
/// <exception cref="ArgumentException">Thrown when id is less than or equal to zero.</exception>
public async Task<User?> GetByIdAsync(int id)
{
    if (id <= 0)
    {
        throw new ArgumentException("ID must be greater than zero", nameof(id));
    }

    return await _context.Users.FindAsync(id);
}
```

### 2. Comentarios

```csharp
// ✅ CORRECTO (comentar el "por qué", no el "qué")
// Using BCrypt with cost factor 11 for better security
// while maintaining acceptable performance (<100ms)
var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, 11);

// ❌ INCORRECTO (obvio, redundante)
// Hash the password
var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
```

## 🚀 Ejecución de Análisis Estático

### Durante el Desarrollo

El análisis se ejecuta automáticamente en build:

```bash
dotnet build
```

### Análisis Completo

```bash
# Análisis con todas las advertencias
dotnet build /p:TreatWarningsAsErrors=true

# Ver warnings como lista
dotnet build /warnaserror

# Análisis con reportes
dotnet build /p:AnalysisLevel=latest
```

### Suprimir Warnings Específicos

Solo cuando sea absolutamente necesario:

```csharp
#pragma warning disable CA1031 // Do not catch general exception types
try
{
    // Code that requires catching all exceptions
}
catch (Exception ex)
{
    // Handle
}
#pragma warning restore CA1031
```

## 📊 Métricas de Calidad

### Objetivos

- **Cobertura de Código**: > 80%
- **Complejidad Ciclomática**: < 10 por método
- **Mantenibilidad**: > 70
- **Code Smells**: 0 (críticos)
- **Vulnerabilidades de Seguridad**: 0

## 🔍 Revisión de Código

### Checklist

Antes de enviar un Pull Request, verificar:

- [ ] Código compila sin errores
- [ ] Código compila sin warnings
- [ ] Todos los tests pasan
- [ ] Cobertura de tests adecuada
- [ ] Nombres siguen convenciones
- [ ] Código está documentado
- [ ] No hay secrets en el código
- [ ] Análisis estático pasa sin issues críticos

## 📖 Referencias

- [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [.NET Design Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/)
- [EditorConfig](https://editorconfig.org/)
- [StyleCop Rules](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/DOCUMENTATION.md)
- [SonarQube C# Rules](https://rules.sonarsource.com/csharp)

---

**Versión**: 1.0
**Última actualización**: Diciembre 2024
**Mantenido por**: Equipo Plantita
