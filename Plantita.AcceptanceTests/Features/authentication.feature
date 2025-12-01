# language: es
Característica: Autenticación de usuarios
  Como usuario de Plantita
  Quiero poder registrarme, iniciar sesión y gestionar mi sesión
  Para acceder de forma segura a la plataforma de cuidado de plantas

  Antecedentes:
    Dado que la API de Plantita está disponible
    Y la base de datos está limpia

  Escenario: Registro exitoso de un nuevo usuario
    Cuando envío una solicitud POST a "/plantita/v1/authentication/sign-up" con:
      | campo    | valor                    |
      | email    | [email protected]  |
      | password | Password123!             |
      | name     | Juan Pérez               |
      | timezone | America/Lima             |
      | language | es                       |
    Entonces debería recibir un código de estado 201
    Y la respuesta debería contener el email "[email protected]"
    Y la respuesta debería contener el nombre "Juan Pérez"
    Y el usuario debería estar guardado en la base de datos

  Escenario: Error al registrar un usuario con email duplicado
    Dado que existe un usuario con email "[email protected]"
    Cuando envío una solicitud POST a "/plantita/v1/authentication/sign-up" con:
      | campo    | valor                    |
      | email    | [email protected]  |
      | password | Password123!             |
      | name     | María López              |
      | timezone | America/Lima             |
      | language | es                       |
    Entonces debería recibir un código de estado 400
    Y la respuesta debería contener un mensaje de error indicando que el usuario ya existe

  Escenario: Error al registrar usuario con email inválido
    Cuando envío una solicitud POST a "/plantita/v1/authentication/sign-up" con:
      | campo    | valor            |
      | email    | email-invalido   |
      | password | Password123!     |
      | name     | Pedro García     |
      | timezone | America/Lima     |
      | language | es               |
    Entonces debería recibir un código de estado 400
    Y la respuesta debería contener un mensaje de error sobre el formato del email

  Escenario: Error al registrar usuario con contraseña débil
    Cuando envío una solicitud POST a "/plantita/v1/authentication/sign-up" con:
      | campo    | valor                |
      | email    | [email protected] |
      | password | 123                  |
      | name     | Ana Martínez         |
      | timezone | America/Lima         |
      | language | es                   |
    Entonces debería recibir un código de estado 400
    Y la respuesta debería contener un mensaje de error sobre la fortaleza de la contraseña

  Escenario: Inicio de sesión exitoso con credenciales válidas
    Dado que existe un usuario registrado con:
      | campo    | valor                    |
      | email    | [email protected]  |
      | password | Password123!             |
      | name     | Carlos Ruiz              |
    Cuando envío una solicitud POST a "/plantita/v1/authentication/sign-in" con:
      | campo    | valor                    |
      | email    | [email protected]  |
      | password | Password123!             |
    Entonces debería recibir un código de estado 200
    Y la respuesta debería contener un token de acceso JWT
    Y la respuesta debería contener un token de actualización
    Y debería recibir una cookie HTTP-only con el refresh token

  Escenario: Error al iniciar sesión con email inexistente
    Cuando envío una solicitud POST a "/plantita/v1/authentication/sign-in" con:
      | campo    | valor                       |
      | email    | [email protected] |
      | password | Password123!                |
    Entonces debería recibir un código de estado 401
    Y la respuesta debería contener un mensaje de error de autenticación

  Escenario: Error al iniciar sesión con contraseña incorrecta
    Dado que existe un usuario registrado con:
      | campo    | valor                    |
      | email    | [email protected]  |
      | password | Password123!             |
      | name     | Laura Torres             |
    Cuando envío una solicitud POST a "/plantita/v1/authentication/sign-in" con:
      | campo    | valor                    |
      | email    | [email protected]  |
      | password | ContraseñaIncorrecta123! |
    Entonces debería recibir un código de estado 401
    Y la respuesta debería contener un mensaje de error de autenticación

  Escenario: Actualización exitosa del token de acceso
    Dado que tengo un refresh token válido para el usuario "[email protected]"
    Cuando envío una solicitud POST a "/plantita/v1/authentication/refresh-token" con el refresh token
    Entonces debería recibir un código de estado 200
    Y la respuesta debería contener un nuevo token de acceso JWT
    Y la respuesta debería contener un nuevo token de actualización
    Y debería recibir una nueva cookie HTTP-only con el refresh token

  Escenario: Error al intentar actualizar con refresh token inválido
    Cuando envío una solicitud POST a "/plantita/v1/authentication/refresh-token" con:
      | campo        | valor              |
      | refreshToken | token_invalido_123 |
    Entonces debería recibir un código de estado 401
    Y la respuesta debería contener un mensaje de error sobre token inválido

  Escenario: Error al intentar actualizar con refresh token expirado
    Dado que tengo un refresh token expirado para el usuario "[email protected]"
    Cuando envío una solicitud POST a "/plantita/v1/authentication/refresh-token" con el refresh token expirado
    Entonces debería recibir un código de estado 401
    Y la respuesta debería contener un mensaje de error sobre token expirado

  Escenario: Cierre de sesión exitoso
    Dado que estoy autenticado como "[email protected]"
    Cuando envío una solicitud POST a "/plantita/v1/authentication/sign-out"
    Entonces debería recibir un código de estado 200
    Y el refresh token debería ser revocado en la base de datos
    Y la cookie de sesión debería ser eliminada

  Escenario: Acceso a recursos protegidos con token válido
    Dado que estoy autenticado con un token JWT válido
    Cuando intento acceder a un recurso protegido con el token en el header Authorization
    Entonces debería poder acceder al recurso exitosamente

  Escenario: Error al acceder a recursos protegidos sin token
    Cuando intento acceder a un recurso protegido sin enviar token de autenticación
    Entonces debería recibir un código de estado 401
    Y la respuesta debería indicar que se requiere autenticación

  Escenario: Error al acceder a recursos protegidos con token expirado
    Dado que tengo un token JWT expirado
    Cuando intento acceder a un recurso protegido con el token expirado
    Entonces debería recibir un código de estado 401
    Y la respuesta debería indicar que el token ha expirado
