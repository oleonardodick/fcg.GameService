using fcg.GameService.Presentation.DTOs.Game.Requests;
using FluentValidation;

namespace fcg.GameService.Application.Validators;

public class PurchaseGameDTOValidator : AbstractValidator<PurchaseGameDTO>
{
    public PurchaseGameDTOValidator()
    {
        RuleFor(g => g.UserId)
            .NotEmpty().WithMessage("O ID do usuário deve ser informado.");

        RuleFor(g => g.GameId)
            .NotEmpty().WithMessage("O ID do jogo deve ser informado.");
    }
}
