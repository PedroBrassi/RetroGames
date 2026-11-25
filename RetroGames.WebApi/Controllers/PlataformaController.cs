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

    [HttpPost]
public async Task<ActionResult<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>>
    Inserir([FromBody] InserirPlataformaRequestApiModel request)
{
    var criada = await _servico.InserirAsync(request.ParaModel());

    var resposta = new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>
    {
        ResponseCode = ResponseCodes.SUCESSO,
        Data = criada.ParaResponse()
    };

    return CreatedAtAction(nameof(ListarPorId), new { id = criada.Id }, resposta);
}

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>>
        Alterar(int id, [FromBody] AlterarPlataformaRequestApiModel request)
    {
        await _servico.AlterarAsync(request.ParaModel(id));
        var alterada = await _servico.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = alterada.ParaResponse()
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Excluir(int id)
    {
        await _servico.ExcluirAsync(id);

        return Ok(new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.SUCESSO
        });
    }
}