using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

public sealed class AlterarJogoRequestValidator
    : JogoRequestValidator<AlterarJogoRequestApiModel>
{
    public AlterarJogoRequestValidator()
    {
        RuleFor(request => request.Ativo)
            .NotNull().WithMessage("Informe se o jogo está ativo.");
    }
}