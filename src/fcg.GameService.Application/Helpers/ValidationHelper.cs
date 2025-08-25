using fcg.GameService.Domain.Models;
using FluentValidation;

namespace fcg.GameService.Application.Helpers;

public class ValidationHelper
{
    public static List<ErrorDetails> Validate<T>(IValidator<T> validator, T request)
    {
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return validationResult.Errors
                .GroupBy(p => p.PropertyName)
                .Select(g => new ErrorDetails
                {
                    Property = g.Key,
                    Errors = g.Select(e => e.ErrorMessage).ToList()
                })
                .ToList();
        }

        return [];
    }

}
