using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

public abstract class JogoRequestValidator<T> : AbstractValidator<T>
    where T : InserirJogoRequestApiModel
{
    protected JogoRequestValidator()
    {
        RuleFor(request => request.Nome)
            .NotEmpty().WithMessage("Informe o nome.")
            .MaximumLength(150).WithMessage("O nome deve ter no máximo 150 caracteres.");

        RuleFor(request => request.Descricao)
            .MaximumLength(1000).WithMessage("A descrição deve ter no máximo 1000 caracteres.");

        RuleFor(request => request.AnoLancamento)
            .InclusiveBetween(1970, 2100).WithMessage("Ano de lançamento inválido.");

        RuleFor(request => request.Preco)
            .GreaterThan(0m).WithMessage("O preço deve ser maior que zero.")
            .PrecisionScale(10, 2, true)
            .WithMessage("O preço deve ter até oito dígitos inteiros e duas casas decimais.");

        RuleFor(request => request.Estoque)
            .GreaterThanOrEqualTo(0).WithMessage("O estoque não pode ser negativo.");

        RuleFor(request => request.IdPlataforma)
            .GreaterThan(0).WithMessage("Informe uma plataforma válida.");

        RuleFor(request => request.IdCondicaoJogo)
            .GreaterThan(0).WithMessage("Informe uma condição válida.");

        RuleFor(request => request.IdsGeneros)
            .NotEmpty().WithMessage("Informe ao menos um gênero.");

        RuleForEach(request => request.IdsGeneros)
            .GreaterThan(0).WithMessage("O ID do gênero deve ser maior que zero.");
    }
}

public sealed class InserirJogoRequestValidator
    : JogoRequestValidator<InserirJogoRequestApiModel>
{
}