# language: es
Característica: Gestión del catálogo de plantas
  Como administrador de la plataforma Plantita
  Quiero gestionar el catálogo de especies de plantas
  Para que los usuarios puedan identificar y agregar plantas a su colección

  Antecedentes:
    Dado que la API de Plantita está disponible
    Y estoy autenticado como administrador

  Escenario: Registrar una nueva planta en el catálogo
    Cuando envío una solicitud POST a "/plantita/v1/plant" con:
      | campo              | valor                                                           |
      | scientificName     | Monstera deliciosa                                              |
      | commonName         | Costilla de Adán                                                |
      | description        | Planta tropical de hojas grandes con perforaciones naturales    |
      | wateringFrequency  | Cada 7-10 días                                                  |
      | sunlightRequirement| Luz indirecta brillante                                         |
      | optimalTemperature | 18-27°C                                                         |
      | humidity           | Alta (60-80%)                                                   |
    Entonces debería recibir un código de estado 201
    Y la respuesta debería contener el nombre científico "Monstera deliciosa"
    Y la respuesta debería contener el nombre común "Costilla de Adán"
    Y la planta debería estar guardada en el catálogo

  Escenario: Consultar todas las plantas del catálogo
    Dado que existen las siguientes plantas en el catálogo:
      | scientificName      | commonName        |
      | Monstera deliciosa  | Costilla de Adán  |
      | Ficus elastica      | Árbol del caucho  |
      | Pothos aureus       | Potus dorado      |
    Cuando envío una solicitud GET a "/plantita/v1/plant"
    Entonces debería recibir un código de estado 200
    Y la respuesta debería contener una lista con 3 plantas
    Y la lista debería incluir "Monstera deliciosa"
    Y la lista debería incluir "Ficus elastica"
    Y la lista debería incluir "Pothos aureus"

  Escenario: Consultar una planta específica por ID
    Dado que existe una planta con ID "123" y nombre "Monstera deliciosa"
    Cuando envío una solicitud GET a "/plantita/v1/plant/123"
    Entonces debería recibir un código de estado 200
    Y la respuesta debería contener el nombre científico "Monstera deliciosa"
    Y la respuesta debería contener todos los detalles de la planta

  Escenario: Error al consultar planta con ID inexistente
    Cuando envío una solicitud GET a "/plantita/v1/plant/99999"
    Entonces debería recibir un código de estado 404
    Y la respuesta debería contener un mensaje de error indicando que la planta no existe

  Escenario: Buscar planta por nombre común
    Dado que existe una planta con nombre común "Costilla de Adán"
    Cuando envío una solicitud GET a "/plantita/v1/plant/common/Costilla de Adán"
    Entonces debería recibir un código de estado 200
    Y la respuesta debería contener el nombre científico "Monstera deliciosa"
    Y la respuesta debería contener el nombre común "Costilla de Adán"

  Escenario: Búsqueda de planta por nombre común inexistente
    Cuando envío una solicitud GET a "/plantita/v1/plant/common/Planta Imaginaria"
    Entonces debería recibir un código de estado 404
    Y la respuesta debería indicar que no se encontró la planta

  Escenario: Actualizar información de una planta existente
    Dado que existe una planta con ID "456" y nombre "Ficus elastica"
    Cuando envío una solicitud PUT a "/plantita/v1/plant/456" con:
      | campo              | valor                                          |
      | scientificName     | Ficus elastica                                 |
      | commonName         | Árbol del caucho                               |
      | description        | Descripción actualizada con más detalles       |
      | wateringFrequency  | Cada 10-14 días                                |
      | sunlightRequirement| Luz indirecta moderada                         |
    Entonces debería recibir un código de estado 200
    Y la respuesta debería contener la descripción actualizada
    Y la respuesta debería contener el nuevo régimen de riego

  Escenario: Error al actualizar planta inexistente
    Cuando envío una solicitud PUT a "/plantita/v1/plant/99999" con datos válidos
    Entonces debería recibir un código de estado 404
    Y la respuesta debería indicar que la planta no existe

  Escenario: Eliminar una planta del catálogo
    Dado que existe una planta con ID "789"
    Cuando envío una solicitud DELETE a "/plantita/v1/plant/789"
    Entonces debería recibir un código de estado 200
    Y la planta no debería existir más en el catálogo
    Y al consultar GET "/plantita/v1/plant/789" debería recibir 404

  Escenario: Error al eliminar planta inexistente
    Cuando envío una solicitud DELETE a "/plantita/v1/plant/99999"
    Entonces debería recibir un código de estado 404
    Y la respuesta debería indicar que la planta no existe

  Escenario: Identificar y guardar planta mediante IA
    Cuando envío una solicitud POST a "/plantita/v1/plant/identify-and-save" con:
      | campo | valor                              |
      | image | base64_encoded_plant_image_data    |
    Entonces debería recibir un código de estado 201
    Y la respuesta debería contener el nombre científico identificado
    Y la respuesta debería contener el nombre común
    Y la planta identificada debería guardarse en el catálogo
    Y la respuesta debería incluir el nivel de confianza de la identificación

  Escenario: Error al identificar planta con imagen inválida
    Cuando envío una solicitud POST a "/plantita/v1/plant/identify-and-save" con:
      | campo | valor              |
      | image | imagen_corrupta    |
    Entonces debería recibir un código de estado 400
    Y la respuesta debería contener un mensaje de error sobre la imagen

  Escenario: Validación de campos requeridos al crear planta
    Cuando envío una solicitud POST a "/plantita/v1/plant" con:
      | campo       | valor                |
      | commonName  | Planta sin nombre    |
    Entonces debería recibir un código de estado 400
    Y la respuesta debería indicar que falta el nombre científico

  Esquema del escenario: Validación de diferentes tipos de plantas
    Cuando registro una planta de tipo "<tipo>" con:
      | scientificName      | <nombreCientifico> |
      | commonName          | <nombreComun>      |
      | wateringFrequency   | <riego>            |
      | sunlightRequirement | <luz>              |
    Entonces debería recibir un código de estado 201
    Y la planta debería tener las características de "<tipo>"

    Ejemplos:
      | tipo       | nombreCientifico    | nombreComun       | riego          | luz                      |
      | Suculenta  | Echeveria elegans   | Echeveria         | Cada 14-21 días| Luz directa              |
      | Tropical   | Calathea ornata     | Calatea           | Cada 5-7 días  | Luz indirecta            |
      | Cactus     | Opuntia ficus       | Nopal             | Cada 21-30 días| Luz directa              |
      | Helecho    | Nephrolepis exaltata| Helecho de Boston | Cada 3-5 días  | Sombra o luz indirecta   |
