# Plantita Tests

Proyecto de pruebas unitarias para la aplicación **Plantita**.

## Descripción

Este proyecto contiene pruebas unitarias completas para los servicios principales de Plantita, una aplicación de gestión de plantas con integración IoT. Las pruebas están implementadas usando **xUnit**, **Moq** y **FluentAssertions**.

## Estructura del Proyecto

```
plantita.Tests/
├── Unit/
│   ├── User/
│   │   └── AuthUserCommandServiceTests.cs
│   └── ProjectPlantita/
│       ├── PlantManagement/
│       │   ├── PlantIdentificationServiceTests.cs
│       │   ├── PlantCommandServiceTests.cs
│       │   └── MyPlantCommandServiceTests.cs
│       └── IoTMonitoring/
│           └── SensorServiceTests.cs
└── plantita.Tests.csproj
```

## Cobertura de Pruebas

### 1. PlantIdentificationServiceTests (6 pruebas)
Pruebas para el servicio de identificación de plantas mediante API externa (Plant.id):
- ✅ Identificación exitosa con imagen válida
- ✅ Manejo de API key faltante
- ✅ Manejo de errores de la API
- ✅ Manejo de respuestas malformadas
- ✅ Manejo de plantas sin nombre común
- ✅ Verificación de headers de autenticación

### 2. PlantCommandServiceTests (9 pruebas)
Pruebas para el servicio de comandos de plantas:
- ✅ Identificación y registro de nueva planta
- ✅ Retorno de planta existente si ya está registrada
- ✅ Manejo de fallo en identificación
- ✅ Registro de nueva planta manual
- ✅ Prevención de duplicados por nombre científico
- ✅ Actualización de planta existente
- ✅ Manejo de ID inválido en actualización
- ✅ Eliminación exitosa de planta
- ✅ Manejo de ID inválido en eliminación

### 3. MyPlantCommandServiceTests (11 pruebas)
Pruebas para el servicio de gestión de plantas del usuario:
- ✅ Creación de planta personal
- ✅ Registro de planta con imagen de catálogo
- ✅ Manejo de planta base no existente
- ✅ Actualización de planta personal
- ✅ Manejo de ID inválido en actualización
- ✅ Eliminación de planta personal
- ✅ Manejo de ID inválido en eliminación
- ✅ Marcado de tarea de cuidado como completada
- ✅ Manejo de tarea inválida
- ✅ Registro de log de salud de planta
- ✅ Generación automática de tareas de cuidado (riego, fertilización)

### 4. AuthUserCommandServiceTests (8 pruebas)
Pruebas para el servicio de autenticación de usuarios:
- ✅ Login exitoso con credenciales válidas
- ✅ Manejo de email inválido en login
- ✅ Manejo de contraseña incorrecta
- ✅ Registro exitoso de nuevo usuario
- ✅ Prevención de registro con email duplicado
- ✅ Manejo de errores de base de datos
- ✅ Asignación de rol por defecto
- ✅ Verificación de hashing de contraseñas

### 5. SensorServiceTests (10 pruebas)
Pruebas para el servicio de gestión de sensores IoT:
- ✅ Listado de todos los sensores
- ✅ Obtención de sensor por ID válido
- ✅ Manejo de ID inválido en consulta
- ✅ Obtención de sensores por dispositivo
- ✅ Creación exitosa de sensor
- ✅ Actualización de sensor existente
- ✅ Manejo de ID inválido en actualización
- ✅ Eliminación exitosa de sensor
- ✅ Manejo de ID inválido en eliminación
- ✅ Verificación de todas las propiedades del sensor

## Total: 44 Pruebas Unitarias

## Tecnologías Utilizadas

- **xUnit 2.8.1**: Framework de pruebas
- **Moq 4.20.70**: Framework para crear mocks
- **FluentAssertions 6.12.0**: Biblioteca para assertions más legibles
- **.NET 8.0**: Framework de desarrollo

## Ejecutar las Pruebas

### Usando dotnet CLI

```bash
# Ejecutar todas las pruebas
dotnet test

# Ejecutar con verbosidad detallada
dotnet test --verbosity detailed

# Ejecutar pruebas específicas
dotnet test --filter "FullyQualifiedName~PlantIdentificationServiceTests"

# Generar reporte de cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### Usando Visual Studio
1. Abrir la solución `plantita.sln`
2. Ir a Test > Run All Tests
3. Ver resultados en Test Explorer

### Usando Rider
1. Abrir la solución `plantita.sln`
2. Click derecho en el proyecto `plantita.Tests`
3. Seleccionar "Run Unit Tests"

## Patrones de Prueba Utilizados

### Arrange-Act-Assert (AAA)
Todas las pruebas siguen el patrón AAA para mayor claridad:
```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange - Configurar datos y mocks
    var mockData = ...;

    // Act - Ejecutar el método bajo prueba
    var result = await service.Method(mockData);

    // Assert - Verificar el resultado
    result.Should().NotBeNull();
}
```

### Mocking de Dependencias
Se usan mocks para aislar las pruebas:
```csharp
var mockRepository = new Mock<IRepository>();
mockRepository
    .Setup(r => r.GetAsync(id))
    .ReturnsAsync(expectedData);
```

### Verificación de Llamadas
Se verifican las interacciones con dependencias:
```csharp
_mockRepository.Verify(r => r.AddAsync(It.IsAny<Entity>()), Times.Once);
```

## Casos de Prueba Cubiertos

### ✅ Casos Exitosos (Happy Path)
- Operaciones CRUD exitosas
- Flujos de negocio completos
- Transformaciones de datos correctas

### ✅ Casos de Error
- Manejo de entradas nulas o inválidas
- Errores de base de datos
- Errores de APIs externas
- Entidades no encontradas

### ✅ Casos de Validación
- Validación de duplicados
- Validación de credenciales
- Validación de permisos
- Validación de estado

### ✅ Casos de Integración
- Interacción con servicios externos (Plant.id API)
- Coordinación entre múltiples repositorios
- Transacciones con UnitOfWork

## Mejores Prácticas Aplicadas

1. **Nombres Descriptivos**: Cada prueba indica claramente qué se está probando
2. **Independencia**: Las pruebas no dependen unas de otras
3. **Determinismo**: Las pruebas siempre producen el mismo resultado
4. **Rapidez**: Uso de mocks para evitar dependencias externas
5. **Cobertura**: Se prueban casos exitosos, de error y límite
6. **Assertions Claras**: Uso de FluentAssertions para mejor legibilidad

## Próximos Pasos

- [ ] Agregar pruebas de integración con base de datos real
- [ ] Implementar pruebas de rendimiento
- [ ] Agregar pruebas end-to-end para controladores
- [ ] Configurar reporte de cobertura de código
- [ ] Agregar pruebas para servicios de IoT restantes
- [ ] Implementar pruebas para el sistema de notificaciones

## Contribuir

Para agregar nuevas pruebas:

1. Crear archivo de prueba en la estructura de carpetas apropiada
2. Seguir el patrón AAA (Arrange-Act-Assert)
3. Usar nombres descriptivos para los métodos de prueba
4. Verificar que todas las pruebas pasen antes de hacer commit
5. Mantener alta cobertura de código

## Licencia

Este proyecto de pruebas sigue la misma licencia que el proyecto principal Plantita.
