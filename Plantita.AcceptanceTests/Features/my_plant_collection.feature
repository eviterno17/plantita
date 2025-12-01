# language: es
Característica: Gestión de la colección personal de plantas
  Como usuario de Plantita
  Quiero gestionar mi colección personal de plantas
  Para hacer seguimiento del cuidado de mis plantas

  Antecedentes:
    Dado que la API de Plantita está disponible
    Y estoy autenticado como "usuario@example.com"
    Y existen las siguientes plantas en el catálogo:
      | id  | scientificName      | commonName       |
      | 1   | Monstera deliciosa  | Costilla de Adán |
      | 2   | Ficus elastica      | Árbol del caucho |
      | 3   | Pothos aureus       | Potus dorado     |

  Escenario: Agregar una planta del catálogo a mi colección
    Cuando envío una solicitud POST a "/plantita/v1/myplant/1" con:
      | campo        | valor                           |
      | customName   | Mi Monstera del salón           |
      | location     | Salón junto a la ventana este   |
      | notes        | Regalo de cumpleaños 2024       |
    Entonces debería recibir un código de estado 201
    Y la respuesta debería contener el nombre personalizado "Mi Monstera del salón"
    Y la respuesta debería contener la ubicación "Salón junto a la ventana este"
    Y la respuesta debería contener las notas
    Y la planta debería estar en mi colección

  Escenario: Error al agregar planta inexistente a mi colección
    Cuando envío una solicitud POST a "/plantita/v1/myplant/99999" con:
      | campo        | valor                    |
      | customName   | Planta fantasma          |
      | location     | Sala                     |
    Entonces debería recibir un código de estado 404
    Y la respuesta debería indicar que la planta no existe en el catálogo

  Escenario: Consultar todas mis plantas
    Dado que tengo las siguientes plantas en mi colección:
      | plantId | customName              | location              |
      | 1       | Monstera del salón      | Salón ventana este    |
      | 2       | Ficus de la oficina     | Oficina escritorio    |
      | 3       | Potus del baño          | Baño ventana norte    |
    Cuando envío una solicitud GET a "/plantita/v1/myplant"
    Entonces debería recibir un código de estado 200
    Y la respuesta debería contener una lista con 3 plantas
    Y la lista debería incluir "Monstera del salón"
    Y la lista debería incluir "Ficus de la oficina"
    Y la lista debería incluir "Potus del baño"

  Escenario: Consultar mis plantas cuando no tengo ninguna
    Dado que no tengo plantas en mi colección
    Cuando envío una solicitud GET a "/plantita/v1/myplant"
    Entonces debería recibir un código de estado 200
    Y la respuesta debería contener una lista vacía

  Escenario: Consultar detalles de una planta específica de mi colección
    Dado que tengo una planta con ID "123" en mi colección
    Cuando envío una solicitud GET a "/plantita/v1/myplant/123"
    Entonces debería recibir un código de estado 200
    Y la respuesta debería contener todos los detalles de la planta
    Y la respuesta debería incluir el nombre científico de la planta del catálogo
    Y la respuesta debería incluir mi nombre personalizado
    Y la respuesta debería incluir la ubicación
    Y la respuesta debería incluir las notas

  Escenario: Error al consultar planta que no está en mi colección
    Cuando envío una solicitud GET a "/plantita/v1/myplant/99999"
    Entonces debería recibir un código de estado 404
    Y la respuesta debería indicar que la planta no existe en mi colección

  Escenario: No puedo ver plantas de otros usuarios
    Dado que el usuario "otro@example.com" tiene una planta con ID "456"
    Cuando envío una solicitud GET a "/plantita/v1/myplant/456"
    Entonces debería recibir un código de estado 403
    Y la respuesta debería indicar que no tengo acceso a esa planta

  Escenario: Agregar una foto a mi planta
    Dado que tengo una planta con ID "123" en mi colección
    Cuando agrego una foto a la planta con:
      | campo     | valor                        |
      | image     | base64_encoded_image_data    |
      | caption   | Foto del primer brote        |
    Entonces debería recibir un código de estado 200
    Y la foto debería estar asociada a mi planta
    Y al consultar la planta debería ver la foto

  Escenario: Registrar una tarea de cuidado para mi planta
    Dado que tengo una planta con ID "123" en mi colección
    Cuando creo una tarea de cuidado con:
      | campo         | valor                      |
      | taskType      | Riego                      |
      | scheduledDate | 2024-12-15                 |
      | notes         | Riego profundo             |
      | status        | Pendiente                  |
    Entonces debería recibir un código de estado 201
    Y la tarea debería estar asociada a mi planta
    Y la tarea debería tener estado "Pendiente"

  Escenario: Marcar tarea de cuidado como completada
    Dado que tengo una planta con ID "123" con una tarea pendiente de riego
    Cuando marco la tarea como completada con fecha "2024-12-15T10:30:00"
    Entonces debería recibir un código de estado 200
    Y la tarea debería tener estado "Completada"
    Y la tarea debería tener la fecha de finalización registrada

  Escenario: Ver tareas de cuidado pendientes de mi planta
    Dado que tengo una planta con ID "123" con las siguientes tareas:
      | taskType    | scheduledDate | status      |
      | Riego       | 2024-12-15    | Pendiente   |
      | Fertilizar  | 2024-12-20    | Pendiente   |
      | Podar       | 2024-12-10    | Completada  |
    Cuando consulto las tareas pendientes de la planta
    Entonces debería ver 2 tareas pendientes
    Y no debería ver la tarea completada de "Podar"

  Escenario: Registrar observación de salud de mi planta
    Dado que tengo una planta con ID "123" en mi colección
    Cuando registro una observación de salud con:
      | campo        | valor                                    |
      | date         | 2024-12-15                               |
      | healthStatus | Buena                                    |
      | observations | Nuevas hojas creciendo, color vibrante   |
    Entonces debería recibir un código de estado 201
    Y la observación debería estar guardada
    Y debería poder consultar el historial de salud de la planta

  Escenario: Ver historial de salud de mi planta
    Dado que tengo una planta con ID "123" con las siguientes observaciones:
      | date       | healthStatus | observations              |
      | 2024-12-01 | Buena        | Planta saludable          |
      | 2024-12-08 | Regular      | Hojas amarillentas        |
      | 2024-12-15 | Buena        | Recuperación exitosa      |
    Cuando consulto el historial de salud de la planta
    Entonces debería ver 3 observaciones ordenadas por fecha
    Y debería poder ver la evolución de la salud de la planta

  Esquema del escenario: Agregar diferentes tipos de plantas a mi colección
    Cuando agrego una planta de tipo "<tipo>" a mi colección con:
      | plantId    | <plantId>    |
      | customName | <customName> |
      | location   | <location>   |
    Entonces debería tener la planta "<customName>" en mi colección
    Y debería poder ver sus características de "<tipo>"

    Ejemplos:
      | tipo       | plantId | customName               | location                 |
      | Suculenta  | 4       | Echeveria de la terraza  | Terraza sur              |
      | Tropical   | 5       | Calatea del dormitorio   | Dormitorio estantería    |
      | Cactus     | 6       | Cactus de la cocina      | Cocina ventana           |
      | Helecho    | 7       | Helecho del baño         | Baño junto a la ducha    |

  Escenario: Eliminar una planta de mi colección
    Dado que tengo una planta con ID "123" en mi colección
    Cuando elimino la planta de mi colección
    Entonces debería recibir un código de estado 200
    Y la planta no debería estar más en mi colección
    Y al consultar la planta debería recibir un 404

  Escenario: Las tareas y observaciones se eliminan al eliminar la planta
    Dado que tengo una planta con ID "123" con tareas y observaciones de salud
    Cuando elimino la planta de mi colección
    Entonces todas las tareas asociadas deberían eliminarse
    Y todas las observaciones de salud deberían eliminarse
