using fcg.GameService.Presentation.DTOs.Game.Requests;
using FluentValidation;

namespace fcg.GameService.Application.Validators;

public class UpdateGameDTOValidator : AbstractValidator<UpdateGameDTO>
{
    public UpdateGameDTOValidator()
    {
        RuleFor(g => g.Name)
               .MaximumLength(50).WithMessage("O nome deve possuir no máximo 50 caracteres.");

        RuleFor(g => g.Description)
            .MaximumLength(500).WithMessage("A descrição deve possuir no máximo 500 caracteres.");

        RuleFor(g => g.Price)
            .GreaterThan(0).WithMessage("O preço deve ser maior que 0.")
            .When(g => g.Price != null);

        RuleFor(g => g.ReleasedDate)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Não é possível cadastrar um jogo com lançamento futuro.")
            .When(g => g.ReleasedDate != null);

        RuleFor(g => g.Tags)
            .ForEach(tag =>
            {
                tag.Must(s => !string.IsNullOrWhiteSpace(s))
                    .WithMessage("A tag não pode ser nula, vazia ou com espaço em branco.");
            })
            .When(g => g.Tags != null);
    }
}
