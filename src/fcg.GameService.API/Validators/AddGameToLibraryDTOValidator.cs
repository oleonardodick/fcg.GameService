using fcg.GameService.API.DTOs.GameLibrary.Requests;
using FluentValidation;

namespace fcg.GameService.API.Validators;

public class AddGameToLibraryDTOValidator:AbstractValidator<AddGameToLibraryDTO>
{
    public AddGameToLibraryDTOValidator()
    {
        RuleFor(g => g.Id)
            .NotEmpty().WithMessage("O ID do jogo deve ser informado.");

        RuleFor(g => g.Name)
            .NotEmpty().WithMessage("O nome do jogo deve ser informado.");
    }
}
