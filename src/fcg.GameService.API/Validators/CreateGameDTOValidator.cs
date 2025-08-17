using fcg.GameService.API.DTOs.Requests;
using FluentValidation;

namespace fcg.GameService.API.Validators;

public class CreateGameDTOValidator:AbstractValidator<CreateGameDTO>
{
    public CreateGameDTOValidator()
    {
        RuleFor(g => g.Name)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(50).WithMessage("O nome deve possuir no máximo 50 caracteres.");

        RuleFor(g => g.Description)
            .MaximumLength(500).WithMessage("A descrição deve possuir no máximo 500 caracteres.");

        RuleFor(g => g.Price)
            .GreaterThan(0).WithMessage("O preço deve ser maior que 0.");

        RuleFor(g => g.ReleasedDate)
            .Must(date => date != default).WithMessage("A data de lançamento deve ser informada.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Não é possível cadastrar um jogo com lançamento futuro.");

        RuleFor(g => g.Tags)
            .NotEmpty().WithMessage("Ao menos uma tag deve ser informada.")
            .ForEach(tag =>
            {
                tag.Must(s => !string.IsNullOrWhiteSpace(s))
                    .WithMessage("A tag não pode ser nula, vazia ou com espaço em branco.");
            });
    }
}
