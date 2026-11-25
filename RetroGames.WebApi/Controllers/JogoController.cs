using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class JogoController : ControllerBase
{
    private readonly IJogoService _servico;

    public JogoController(IJogoService servico)
    {
        _servico = servico;
    }

    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>>>
        Listar([FromQuery] ListarJogoRequestApiModel request)
    {
        var filtro = request.ParaFiltro();

        var (jogos, total) = await _servico.ListarAsync(
            filtro.Pagina,
            filtro.TamanhoPagina,
            filtro.Busca,
            filtro.IdPlataforma,
            filtro.IdGenero);

        var pagina = new PaginaResponseApiModel<ListarJogoResponseApiModel>(
            jogos.Select(jogo => jogo.ParaResponse()).ToArray(),
            filtro.Pagina,
            filtro.TamanhoPagina,
            total,
            (int)Math.Ceiling(total / (double)filtro.TamanhoPagina));

        return Ok(new RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = pagina
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>>
        ListarPorId(int id)
    {
        var jogo = await _servico.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = jogo.ParaResponseDetalhe()
        });
    }

    [HttpPost]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>>
        Inserir([FromBody] InserirJogoRequestApiModel request)
    {
        var criado = await _servico.InserirAsync(request.ParaModel(), request.IdsGeneros);

        var resposta = new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = criado.ParaResponseDetalhe()
        };

        return CreatedAtAction(nameof(ListarPorId), new { id = criado.Jogo.Id }, resposta);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>>
        Alterar(int id, [FromBody] AlterarJogoRequestApiModel request)
    {
        await _servico.AlterarAsync(request.ParaModel(id), request.IdsGeneros);
        var alterado = await _servico.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = alterado.ParaResponseDetalhe()
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Excluir(int id)
    {
        await _servico.DesativarAsync(id);

        return Ok(new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.SUCESSO
        });
    }
}