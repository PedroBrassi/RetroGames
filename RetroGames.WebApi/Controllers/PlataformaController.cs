using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PlataformaController : ControllerBase
{
    private readonly IPlataformaService _servico;

    public PlataformaController(IPlataformaService servico)
    {
        _servico = servico;
    }

    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel[]>>>
        Listar()
    {
        var plataformas = await _servico.ListarAsync();

        return Ok(new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel[]>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = plataformas.Select(plataforma => plataforma.ParaResponse()).ToArray()
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>>
        ListarPorId(int id)
    {
        var plataforma = await _servico.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = plataforma.ParaResponse()
        });
    }
}