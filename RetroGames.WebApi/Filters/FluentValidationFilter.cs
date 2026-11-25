using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Filters;

public sealed class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext contexto,
        ActionExecutionDelegate proximo)
    {
        var erros = new List<(string Campo, string Mensagem)>();

        foreach (var argumento in contexto.ActionArguments.Values)
        {
            if (argumento is null)
            {
                continue;
            }

            var tipoValidator = typeof(IValidator<>).MakeGenericType(argumento.GetType());
            var servicos = contexto.HttpContext.RequestServices;

            if (servicos.GetService(tipoValidator) is IValidator validator)
            {
                var resultado = await validator.ValidateAsync(
                    new ValidationContext<object>(argumento),
                    contexto.HttpContext.RequestAborted);

                erros.AddRange(resultado.Errors.Select(erro => (erro.PropertyName, erro.ErrorMessage)));
            }
        }

        if (erros.Count > 0)
        {
            contexto.Result = CriarResposta(erros);
            return;
        }

        await proximo();
    }

    public static BadRequestObjectResult CriarResposta(
        IEnumerable<(string Campo, string Mensagem)> erros)
    {
        var dados = erros
            .GroupBy(erro => NormalizarCampo(erro.Campo))
            .ToDictionary(
                grupo => grupo.Key,
                grupo => grupo.Select(erro => erro.Mensagem).Distinct().ToArray());

        return new BadRequestObjectResult(new RetroGamesResponseApiModel<Dictionary<string, string[]>>
        {
            ResponseCode = ResponseCodes.MODELO_INVALIDO,
            ErrorMessage = "Os dados enviados são inválidos.",
            Data = dados
        });
    }

    private static string NormalizarCampo(string campo)
    {
        if (string.IsNullOrEmpty(campo) || campo == "$")
        {
            return "corpo";
        }

        var nome = campo.StartsWith("$.") ? campo[2..] : campo;
        return JsonNamingPolicy.CamelCase.ConvertName(nome);
    }
}