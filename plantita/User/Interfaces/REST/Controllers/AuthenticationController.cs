using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using plantita.User.Application.Internal.OutboundServices;
    using plantita.User.Domain.Model.Aggregates;
    using plantita.User.Domain.Model.Commands;
    using plantita.User.Domain.Repositories;
    using plantita.User.Domain.Services;
    using plantita.User.Interfaces.REST.Resources;
    using plantita.User.Interfaces.REST.Transform;

    namespace plantita.User.Interfaces.REST.Controllers;

    [Authorize]
    [ApiController]
    [Route("plantita/v1/authentication")]
    [Produces(MediaTypeNames.Application.Json)]
    public class AuthenticationController(
        IAuthUserCommandService userCommandService,
        IAuthUserRefreshTokenRepository authUserRefreshTokenRepository,
        IAuthUserRepository authUserRepository,
        ITokenService tokenService) : ControllerBase  
        
    {
        /**
         * <summary>
         *     Sign in endpoint. It allows to authenticate a user
         * </summary>
         * <param name="signInResource">The sign in resource containing email and password.</param>
         * <returns>The authenticated user resource, including a JWT token</returns>
         */
        [HttpPost("sign-in")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] SignInResource signInResource)
        {
            var user = await authUserRepository.FindByEmailAsync(signInResource.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(signInResource.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
            }
            

            var jwtToken = tokenService.GenerateToken(user);
            var refreshToken = tokenService.GenerateRefreshToken();

            await tokenService.StoreRefreshToken(user.Id, refreshToken);

            Response.Cookies.Append("AuthToken", jwtToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, 
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(30),

            });

            Response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, 
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7),


            });

            return Ok(new { message = "Inicio de sesión exitoso",jwtToken = jwtToken,userId = user.Id });
        }

       
        [HttpPost("sign-up")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] SignUpResource signUpResource)
        {
            var existingUser = await authUserRepository.FindByEmailAsync(signUpResource.Email);

            if (existingUser != null)
            {
                return BadRequest(new { message = "El usuario ya existe" });
            }

            var signUpCommand = new SignUpCommand(
                signUpResource.Email,
                signUpResource.Password,
                signUpResource.Name,
                signUpResource.LastName,
                signUpResource.timeZone,
                signUpResource.preferredLanguage,
                DateTime.UtcNow,
                "User"
            );

            await userCommandService.Handle(signUpCommand);

            return Ok(new { message = "Usuario creado exitosamente." });
        }

        /**
         * <summary>
         *     Refresh token endpoint. It generates a new access token using a valid refresh token
         * </summary>
         * <param name="request">The refresh token request.</param>
         * <returns>A new access token and refresh token</returns>
         */
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["RefreshToken"];
            Console.WriteLine($"🔎 Valor de RefreshToken recibido: {refreshToken}");

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { message = "No hay refresh token disponible" });
            }

            var storedToken = await authUserRefreshTokenRepository.GetByTokenAsync(refreshToken);
            if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }

            var user = await authUserRepository.FindByIdAsync(storedToken.UserId);
            if (user == null)
            {
                return Unauthorized(new { message = "Usuario no encontrado" });
            }

            var newJwtToken = tokenService.GenerateToken(user);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            await tokenService.StoreRefreshToken(user.Id, newRefreshToken);

            // 🔹 Reemplazar las cookies antiguas con las nuevas
            Response.Cookies.Append("AuthToken", newJwtToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // ⚠️ Solo para desarrollo (true en producción HTTPS)
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(30),

            });

            Response.Cookies.Append("RefreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // ⚠️ Solo para desarrollo (true en producción HTTPS)
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7),

            });

            return Ok(new { message = "Token renovado exitosamente" });
        }


        /**
         * <summary>
         *     Logout endpoint. It invalidates the refresh token to log out the user.
         * </summary>
         * <param name="request">The logout request containing the refresh token.</param>
         * <returns>A confirmation message on successful logout.</returns>
         */
        [HttpPost("sign-out")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["RefreshToken"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                await tokenService.RevokeRefreshToken(refreshToken);
            }

            // 🔹 Eliminar las cookies de sesión
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("RefreshToken");

            return Ok(new { message = "Logout exitoso" });
        }

    }
