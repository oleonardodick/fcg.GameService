using fcg.GameService.API.DTOs;
using FluentValidation;

namespace fcg.GameService.API.Helpers;

public class ValidationHelper
{
    public static List<ErrorResponseDTO> Validate<T>(IValidator<T> validator, T request)
    {
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return validationResult.Errors
                .GroupBy(p => p.PropertyName)
                .Select(g => new ErrorResponseDTO
                {
                    Property = g.Key,
                    Errors = g.Select(e => e.ErrorMessage).ToList()
                })
                .ToList();
        }

        return [];
    }

}
