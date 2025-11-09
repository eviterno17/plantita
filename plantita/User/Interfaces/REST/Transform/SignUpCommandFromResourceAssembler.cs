using plantita.User.Domain.Model.Commands;
using plantita.User.Interfaces.REST.Resources;

namespace plantita.User.Interfaces.REST.Transform;

public static class SignUpCommandFromResourceAssembler
{
    public static SignUpCommand ToCommandFromResource(SignUpResource resource)
    {
        string role = string.IsNullOrEmpty(resource.Role) ? "User" : resource.Role; // Asignar rol por defecto

        return new SignUpCommand(resource.Email, resource.Password,resource.Name,resource.LastName,resource.timeZone,resource.preferredLanguage,DateTime.UtcNow,role);
    }
}