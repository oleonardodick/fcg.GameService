using fcg.GameService.API.DTOs.GameLibrary;
using FluentValidation;

namespace fcg.GameService.API.Validators;

public class CreateGameLibraryDTOValidator:AbstractValidator<CreateGameLibraryDTO>
{
    public CreateGameLibraryDTOValidator()
    {
        RuleFor(l => l.UserId)
            .NotEmpty().WithMessage("O ID do usuário deve ser informado.");
    }
}
