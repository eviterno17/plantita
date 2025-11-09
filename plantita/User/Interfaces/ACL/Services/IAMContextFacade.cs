using plantita.User.Domain.Model.Commands;
using plantita.User.Domain.Model.Queries;
using plantita.User.Domain.Services;

namespace plantita.User.Interfaces.ACL.Services;

public class IamContextFacade(IAuthUserCommandService userCommandService, IAuthUserQueryService userQueryService) : IIamContextFacade
{
    public async Task<Guid> CreateAuthUser(string email, string password,string name,string lastname,string timeZone,string preferredLanguage,DateTime dateCreated,string role)
    {
        var signUpCommand = new SignUpCommand(email, password,name,lastname,timeZone,preferredLanguage,dateCreated,role);
        await userCommandService.Handle(signUpCommand);
        var getUserByUsernameQuery = new GetAuthUserByEmailQuery(email);
        var result = await userQueryService.Handle(getUserByUsernameQuery);
        return result?.Id ?? Guid.Empty;

    }

    public async Task<Guid> FetchAuthUserIdByEmail(string email)
    {
        var getAuthUserByUsernameQuery = new GetAuthUserByEmailQuery(email);
        var result = await userQueryService.Handle(getAuthUserByUsernameQuery);
        return result?.Id ?? Guid.Empty;
    }

    public async Task<string> FetchAuthUsernameByUserId(Guid userId)
    {
        var getAuthUserByIdQuery = new GetAuthUserByIdQuery(userId);
        var result = await userQueryService.Handle(getAuthUserByIdQuery);
        return result?.Email ?? string.Empty;
    }
}