using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GeneroController : ControllerBase
{
    private readonly IGeneroService _servico;

    public GeneroController(IGeneroService servico)
    {
        _servico = servico;
    }

    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarGeneroResponseApiModel[]>>>
        Listar()
    {
        var generos = await _servico.ListarAsync();

        return Ok(new RetroGamesResponseApiModel<ListarGeneroResponseApiModel[]>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = generos.Select(genero => genero.ParaResponse()).ToArray()
        });
    }
}