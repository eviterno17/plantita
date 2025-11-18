# language: es
Característica: Monitoreo de sensores y lecturas ambientales
  Como usuario de Plantita
  Quiero gestionar sensores y recopilar datos ambientales
  Para monitorear las condiciones de mis plantas en tiempo real

  Antecedentes:
    Dado que la API de Plantita está disponible
    Y estoy autenticado como "usuario@example.com"
    Y tengo un dispositivo IoT con ID "100" registrado

  Escenario: Registrar un sensor de temperatura en un dispositivo
    Cuando envío una solicitud POST a "/plantita/v1/sensor" con:
      | campo          | valor              |
      | deviceId       | 100                |
      | sensorType     | Temperature        |
      | unit           | Celsius            |
      | minRange       | -10                |
      | maxRange       | 50                 |
      | isActive       | true               |
    Entonces debería recibir un código de estado 201
    Y la respuesta debería contener el tipo de sensor "Temperature"
    Y la respuesta debería contener la unidad "Celsius"
    Y el sensor debería estar asociado al dispositivo "100"

  Escenario: Registrar un sensor de humedad del suelo
    Cuando envío una solicitud POST a "/plantita/v1/sensor" con:
      | campo          | valor              |
      | deviceId       | 100                |
      | sensorType     | SoilMoisture       |
      | unit           | Percentage         |
      | minRange       | 0                  |
      | maxRange       | 100                |
      | isActive       | true               |
    Entonces debería recibir un código de estado 201
    Y el sensor debería estar listo para recopilar datos de humedad del suelo

  Escenario: Registrar un sensor de luz
    Cuando envío una solicitud POST a "/plantita/v1/sensor" con:
      | campo          | valor              |
      | deviceId       | 100                |
      | sensorType     | Light              |
      | unit           | Lux                |
      | minRange       | 0                  |
      | maxRange       | 100000             |
      | isActive       | true               |
    Entonces debería recibir un código de estado 201
    Y el sensor debería poder medir niveles de luz

  Escenario: Consultar todos los sensores de un dispositivo
    Dado que el dispositivo "100" tiene los siguientes sensores:
      | sensorType      | unit       | isActive |
      | Temperature     | Celsius    | true     |
      | SoilMoisture    | Percentage | true     |
      | Light           | Lux        | true     |
      | AirHumidity     | Percentage | false    |
    Cuando envío una solicitud GET a "/plantita/v1/sensor/device/100"
    Entonces debería recibir un código de estado 200
    Y la respuesta debería contener 4 sensores
    Y debería ver sensores de tipo "Temperature", "SoilMoisture", "Light" y "AirHumidity"

  Escenario: Consultar detalles de un sensor específico
    Dado que existe un sensor con ID "500"
    Cuando envío una solicitud GET a "/plantita/v1/sensor/500"
    Entonces debería recibir un código de estado 200
    Y la respuesta debería contener el tipo de sensor
    Y debería incluir la unidad de medida
    Y debería incluir el rango de valores
    Y debería incluir el estado activo/inactivo

  Escenario: Actualizar configuración de un sensor
    Dado que tengo un sensor con ID "500"
    Cuando envío una solicitud PUT a "/plantita/v1/sensor/500" con:
      | campo      | valor      |
      | minRange   | 0          |
      | maxRange   | 40         |
      | isActive   | true       |
    Entonces debería recibir un código de estado 200
    Y el sensor debería tener el nuevo rango configurado

  Escenario: Desactivar un sensor
    Dado que tengo un sensor activo con ID "500"
    Cuando actualizo el sensor estableciendo isActive en false
    Entonces debería recibir un código de estado 200
    Y el sensor debería estar inactivo
    Y no debería registrar nuevas lecturas

  Escenario: Eliminar un sensor
    Dado que tengo un sensor con ID "500"
    Cuando envío una solicitud DELETE a "/plantita/v1/sensor/500"
    Entonces debería recibir un código de estado 200
    Y el sensor no debería existir más
    Y todas las lecturas asociadas deberían eliminarse

  Escenario: Registrar una lectura de temperatura
    Dado que tengo un sensor de temperatura con ID "500"
    Cuando envío una solicitud POST a "/plantita/v1/sensorreading" con:
      | campo         | valor               |
      | sensorId      | 500                 |
      | value         | 23.5                |
      | timestamp     | 2024-12-15T10:30:00 |
    Entonces debería recibir un código de estado 201
    Y la lectura debería estar guardada
    Y debería incluir el valor 23.5
    Y debería incluir la marca de tiempo

  Escenario: Registrar lectura fuera del rango configurado
    Dado que tengo un sensor con rango de -10 a 50
    Cuando registro una lectura con valor 75
    Entonces debería recibir un código de estado 201
    Y la lectura debería guardarse con una advertencia
    Y debería marcarse como fuera de rango

  Escenario: Consultar lecturas de un sensor específico
    Dado que el sensor "500" tiene las siguientes lecturas:
      | value | timestamp           |
      | 22.5  | 2024-12-15T08:00:00 |
      | 23.0  | 2024-12-15T09:00:00 |
      | 23.5  | 2024-12-15T10:00:00 |
      | 24.0  | 2024-12-15T11:00:00 |
    Cuando envío una solicitud GET a "/plantita/v1/sensorreading/500/sensorID"
    Entonces debería recibir un código de estado 200
    Y debería ver 4 lecturas ordenadas por tiempo
    Y debería poder ver la evolución de la temperatura

  Escenario: Consultar lecturas recientes de un sensor
    Dado que el sensor "500" tiene lecturas de los últimos 7 días
    Cuando consulto las lecturas de las últimas 24 horas
    Entonces debería recibir solo las lecturas del último día
    Y deberían estar ordenadas cronológicamente

  Escenario: Ingesta masiva de datos de sensores
    Dado que tengo múltiples sensores en un dispositivo
    Cuando envío una solicitud POST a "/api/v1/environment/data-records" con:
      | deviceId | sensorReadings                                                    |
      | 100      | [{sensorId:500,value:23.5},{sensorId:501,value:65},{sensorId:502,value:8000}] |
    Entonces debería recibir un código de estado 201
    Y todas las lecturas deberían guardarse en la base de datos
    Y debería recibir confirmación de ingesta exitosa

  Escenario: Calcular promedios de lecturas
    Dado que el sensor "500" tiene las siguientes lecturas:
      | value |
      | 20.0  |
      | 22.0  |
      | 24.0  |
      | 26.0  |
    Cuando consulto las estadísticas del sensor
    Entonces el promedio debería ser 23.0
    Y el valor mínimo debería ser 20.0
    Y el valor máximo debería ser 26.0

  Escenario: Validación de tipos de sensores soportados
    Cuando intento registrar un sensor con tipo no válido "InvalidType"
    Entonces debería recibir un código de estado 400
    Y la respuesta debería indicar los tipos válidos: Temperature, SoilMoisture, Light, AirHumidity

  Escenario: No puedo ver sensores de dispositivos de otros usuarios
    Dado que el usuario "otro@example.com" tiene un dispositivo con ID "200"
    Y ese dispositivo tiene sensores registrados
    Cuando intento consultar los sensores del dispositivo "200"
    Entonces debería recibir un código de estado 403
    Y la respuesta debería indicar que no tengo acceso

  Esquema del escenario: Registrar diferentes tipos de sensores
    Cuando registro un sensor de tipo "<sensorType>" con unidad "<unit>"
    Entonces debería medir "<measurement>"
    Y el rango típico debería ser de <min> a <max>

    Ejemplos:
      | sensorType    | unit       | measurement          | min    | max    |
      | Temperature   | Celsius    | Temperatura del aire | -10    | 50     |
      | SoilMoisture  | Percentage | Humedad del suelo    | 0      | 100    |
      | Light         | Lux        | Intensidad lumínica  | 0      | 100000 |
      | AirHumidity   | Percentage | Humedad del aire     | 0      | 100    |

  Escenario: Actualizar lectura incorrecta
    Dado que existe una lectura con ID "1000" con valor incorrecto
    Cuando envío una solicitud PUT a "/plantita/v1/sensorreading/1000" con:
      | campo | valor |
      | value | 24.5  |
    Entonces debería recibir un código de estado 200
    Y la lectura debería tener el valor corregido

  Escenario: Eliminar lectura errónea
    Dado que existe una lectura errónea con ID "1000"
    Cuando envío una solicitud DELETE a "/plantita/v1/sensorreading/1000"
    Entonces debería recibir un código de estado 200
    Y la lectura no debería existir más

  Escenario: Generar reporte de condiciones ambientales
    Dado que tengo un dispositivo con sensores de temperatura, humedad y luz
    Y hay lecturas de los últimos 7 días
    Cuando solicito un reporte de condiciones ambientales
    Entonces debería recibir un resumen con:
      | métrica                    | descripción                           |
      | Temperatura promedio       | Media de temperatura del período      |
      | Humedad promedio           | Media de humedad del período          |
      | Horas de luz               | Total de horas con luz adecuada       |
      | Condiciones óptimas        | Porcentaje de tiempo en rango óptimo  |
      | Alertas                    | Número de veces fuera de rango        |

  Escenario: Configurar alertas basadas en umbrales de sensores
    Dado que tengo un sensor de humedad del suelo con ID "500"
    Cuando configuro una alerta para valores menores a 30%
    Entonces debería recibir confirmación
    Y cuando la humedad caiga por debajo del 30% debería generarse una alerta

  Escenario: Consultar historial completo de un sensor
    Dado que el sensor "500" ha estado activo por 30 días
    Cuando consulto el historial completo
    Entonces debería recibir todas las lecturas
    Y deberían estar paginadas correctamente
    Y debería poder navegar por páginas de resultados
