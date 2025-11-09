using plantita.User.Interfaces.REST.Resources;

namespace plantita.User.Interfaces.REST.Transform;

public static class AuthUserResourceFromEntityAssembler
{
    public static AuthUserResource ToResourceFromEntity(Domain.Model.Aggregates.AuthUser user)
    {
        return new AuthUserResource(user.Id, user.Email,user.Name,user.LastName,user.TimeZone,user.PreferredLanguage);
    }
}