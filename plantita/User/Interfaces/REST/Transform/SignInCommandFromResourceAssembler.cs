using plantita.User.Domain.Model.Commands;
using plantita.User.Interfaces.REST.Resources;

namespace plantita.User.Interfaces.REST.Transform;

public static class SignInCommandFromResourceAssembler
{
    public static SignInCommand ToCommandFromResource(SignInResource resource)
    {
        return new SignInCommand(resource.Email, resource.Password);
    }
}