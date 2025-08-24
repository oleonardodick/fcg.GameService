using fcg.GameService.Presentation.DTOs.GameLibrary.Requests;
using FluentValidation;

namespace fcg.GameService.Application.Validators;

public class CreateGameLibraryDTOValidator:AbstractValidator<CreateGameLibraryDTO>
{
    public CreateGameLibraryDTOValidator()
    {
        RuleFor(l => l.UserId)
            .NotEmpty().WithMessage("O ID do usuário deve ser informado.");
    }
}
