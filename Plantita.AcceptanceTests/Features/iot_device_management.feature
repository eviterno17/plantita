# language: es
Característica: Gestión de dispositivos IoT
  Como usuario de Plantita
  Quiero gestionar dispositivos IoT de monitoreo
  Para supervisar las condiciones ambientales de mis plantas

  Antecedentes:
    Dado que la API de Plantita está disponible
    Y estoy autenticado como "usuario@example.com"

  Escenario: Registrar un nuevo dispositivo IoT
    Cuando envío una solicitud POST a "/plantita/v1/iotdevice" con:
      | campo          | valor                                  |
      | name           | Monitor Sala Principal                 |
      | connectionType | WiFi                                   |
      | location       | Sala de estar                          |
      | firmwareVersion| v1.2.3                                 |
      | isActive       | true                                   |
    Entonces debería recibir un código de estado 201
    Y la respuesta debería contener el nombre "Monitor Sala Principal"
    Y la respuesta debería contener el tipo de conexión "WiFi"
    Y el dispositivo debería estar asociado a mi usuario
    Y el dispositivo debería estar activo

  Escenario: Consultar todos mis dispositivos IoT
    Dado que tengo los siguientes dispositivos registrados:
      | name                  | connectionType | location        | isActive |
      | Monitor Sala          | WiFi           | Sala de estar   | true     |
      | Sensor Terraza        | Bluetooth      | Terraza         | true     |
      | Monitor Dormitorio    | WiFi           | Dormitorio      | false    |
    Cuando envío una solicitud GET a "/plantita/v1/iotdevice/me/me"
    Entonces debería recibir un código de estado 200
    Y la respuesta debería contener 3 dispositivos
    Y debería ver "Monitor Sala"
    Y debería ver "Sensor Terraza"
    Y debería ver "Monitor Dormitorio"

  Escenario: Consultar detalles de un dispositivo específico
    Dado que tengo un dispositivo con ID "123"
    Cuando envío una solicitud GET a "/plantita/v1/iotdevice/123"
    Entonces debería recibir un código de estado 200
    Y la respuesta debería contener todos los detalles del dispositivo
    Y debería incluir el nombre del dispositivo
    Y debería incluir el tipo de conexión
    Y debería incluir la ubicación
    Y debería incluir la versión del firmware

  Escenario: Error al consultar dispositivo inexistente
    Cuando envío una solicitud GET a "/plantita/v1/iotdevice/99999"
    Entonces debería recibir un código de estado 404
    Y la respuesta debería indicar que el dispositivo no existe

  Escenario: No puedo ver dispositivos de otros usuarios
    Dado que el usuario "otro@example.com" tiene un dispositivo con ID "456"
    Cuando envío una solicitud GET a "/plantita/v1/iotdevice/456"
    Entonces debería recibir un código de estado 403
    Y la respuesta debería indicar que no tengo acceso a ese dispositivo

  Escenario: Actualizar configuración de un dispositivo
    Dado que tengo un dispositivo con ID "123"
    Cuando envío una solicitud PUT a "/plantita/v1/iotdevice/123" con:
      | campo          | valor                  |
      | name           | Monitor Sala Actualizado|
      | location       | Sala esquina norte     |
      | firmwareVersion| v1.3.0                 |
      | isActive       | true                   |
    Entonces debería recibir un código de estado 200
    Y la respuesta debería contener el nombre actualizado
    Y la respuesta debería contener la nueva ubicación
    Y la respuesta debería contener la versión del firmware actualizada

  Escenario: Desactivar un dispositivo IoT
    Dado que tengo un dispositivo activo con ID "123"
    Cuando actualizo el dispositivo estableciendo isActive en false
    Entonces debería recibir un código de estado 200
    Y el dispositivo debería estar inactivo
    Y no debería recibir más lecturas de sensores de este dispositivo

  Escenario: Reactivar un dispositivo IoT desactivado
    Dado que tengo un dispositivo inactivo con ID "123"
    Cuando actualizo el dispositivo estableciendo isActive en true
    Entonces debería recibir un código de estado 200
    Y el dispositivo debería estar activo
    Y debería poder recibir lecturas de sensores

  Escenario: Eliminar un dispositivo IoT
    Dado que tengo un dispositivo con ID "123"
    Cuando envío una solicitud DELETE a "/plantita/v1/iotdevice/123"
    Entonces debería recibir un código de estado 200
    Y el dispositivo no debería existir más
    Y al consultar el dispositivo debería recibir 404

  Escenario: Al eliminar dispositivo se eliminan sus sensores asociados
    Dado que tengo un dispositivo con ID "123" con 3 sensores asociados
    Cuando elimino el dispositivo
    Entonces todos los sensores asociados deberían eliminarse
    Y todas las lecturas de esos sensores deberían eliminarse

  Escenario: Validación de campos requeridos al crear dispositivo
    Cuando envío una solicitud POST a "/plantita/v1/iotdevice" con:
      | campo          | valor     |
      | location       | Sala      |
    Entonces debería recibir un código de estado 400
    Y la respuesta debería indicar que faltan campos requeridos

  Escenario: Error al actualizar dispositivo de otro usuario
    Dado que el usuario "otro@example.com" tiene un dispositivo con ID "456"
    Cuando intento actualizar el dispositivo "456"
    Entonces debería recibir un código de estado 403
    Y la respuesta debería indicar que no tengo permisos

  Escenario: Error al eliminar dispositivo de otro usuario
    Dado que el usuario "otro@example.com" tiene un dispositivo con ID "456"
    Cuando intento eliminar el dispositivo "456"
    Entonces debería recibir un código de estado 403
    Y la respuesta debería indicar que no tengo permisos

  Esquema del escenario: Registrar dispositivos con diferentes tipos de conexión
    Cuando registro un dispositivo con tipo de conexión "<connectionType>"
    Entonces el dispositivo debería soportar el protocolo "<protocol>"
    Y debería poder enviar datos mediante "<method>"

    Ejemplos:
      | connectionType | protocol      | method           |
      | WiFi           | HTTP/MQTT     | API REST/MQTT    |
      | Bluetooth      | BLE           | Bluetooth LE     |
      | Zigbee         | Zigbee        | Mesh Network     |
      | LoRa           | LoRaWAN       | Long Range       |

  Escenario: Verificar estado de conexión del dispositivo
    Dado que tengo un dispositivo con ID "123"
    Cuando consulto el estado de conexión del dispositivo
    Entonces debería recibir información sobre:
      | campo           | descripción                      |
      | lastSeen        | Última comunicación              |
      | isOnline        | Estado actual de conexión        |
      | signalStrength  | Fuerza de señal (si aplica)     |
      | batteryLevel    | Nivel de batería (si aplica)    |

  Escenario: Consultar dispositivos activos solamente
    Dado que tengo 5 dispositivos registrados
    Y 3 están activos y 2 están inactivos
    Cuando filtro por dispositivos activos
    Entonces debería ver solo los 3 dispositivos activos

  Escenario: Actualizar firmware del dispositivo
    Dado que tengo un dispositivo con firmware "v1.0.0"
    Cuando actualizo el firmware a versión "v2.0.0"
    Entonces debería recibir confirmación de actualización
    Y el dispositivo debería mostrar la versión "v2.0.0"
    Y debería registrarse en el historial de actualizaciones
