using fcg.GameService.API.DTOs.Requests;
using FluentValidation;

namespace fcg.GameService.API.Validators;

public class UpdateTagsDTOValidator:AbstractValidator<UpdateTagsDTO>
{
    public UpdateTagsDTOValidator()
    {
        RuleFor(g => g.Tags)
            .NotEmpty().WithMessage("Ao menos uma tag deve ser informada.")
            .ForEach(tag =>
            {
                tag.Must(s => !string.IsNullOrWhiteSpace(s))
                    .WithMessage("A tag não pode ser nula, vazia ou com espaço em branco.");
            });
    }
}
