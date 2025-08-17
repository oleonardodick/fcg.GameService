using fcg.GameService.API.DTOs.GameLibrary.Requests;
using FluentValidation;

namespace fcg.GameService.API.Validators;

public class RemoveGameFromLibraryDTOValidator:AbstractValidator<RemoveGameFromLibraryDTO>
{
    public RemoveGameFromLibraryDTOValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty().WithMessage("O ID do jogo deve ser informado.");
    }
}
