using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Middlewares;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext contexto,
        Exception excecao,
        CancellationToken cancellationToken)
    {
        int status;
        int codigo;
        string mensagem;

        if (excecao is BusinessRuleException erroDeNegocio)
        {
            status = (int)erroDeNegocio.Erro.Status;
            codigo = erroDeNegocio.Erro.Codigo;
            mensagem = erroDeNegocio.Message;
        }
        else
        {
            _logger.LogError(excecao, "Erro inesperado ao processar a requisição.");

            status = StatusCodes.Status500InternalServerError;
            codigo = ResponseCodes.ERRO;
            mensagem = "Ocorreu um erro interno ao processar a requisição.";
        }

        contexto.Response.StatusCode = status;

        await contexto.Response.WriteAsJsonAsync(
            new RetroGamesResponseApiModel
            {
                ResponseCode = codigo,
                ErrorMessage = mensagem
            },
            cancellationToken: cancellationToken);

        return true;
    }
}