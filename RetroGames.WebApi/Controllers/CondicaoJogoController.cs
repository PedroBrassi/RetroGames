using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CondicaoJogoController : ControllerBase
{
    private readonly ICondicaoJogoService _servico;

    public CondicaoJogoController(ICondicaoJogoService servico)
    {
        _servico = servico;
    }

    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarCondicaoJogoResponseApiModel[]>>>
        Listar()
    {
        var condicoes = await _servico.ListarAsync();

        return Ok(new RetroGamesResponseApiModel<ListarCondicaoJogoResponseApiModel[]>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = condicoes.Select(condicao => condicao.ParaResponse()).ToArray()
        });
    }
}