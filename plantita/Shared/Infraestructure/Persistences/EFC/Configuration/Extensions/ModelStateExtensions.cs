using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace plantita.Shared.Infraestructure.Persistences.EFC.Configuration.Extensions;

public static class ModelStateExtensions
{
    public static List<string> GetErrorMessages(this ModelStateDictionary dictionary)
    {
        return dictionary
            .SelectMany(m => m.Value!.Errors)
            .Select(m => m.ErrorMessage)
            .ToList();
    }
    
}