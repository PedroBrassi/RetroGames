using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.Model.Model;

namespace RetroGames.WebApi.Mappers;

public static class JogoMapper
{
    public static (int Pagina, int TamanhoPagina, string? Busca, int? IdPlataforma, int? IdGenero)
        ParaFiltro(this ListarJogoRequestApiModel request)
    {
        var busca = string.IsNullOrWhiteSpace(request.Busca) ? null : request.Busca.Trim();

        return (request.Pagina, request.TamanhoPagina, busca, request.IdPlataforma, request.IdGenero);
    }

    public static ListarJogoResponseApiModel ParaResponse(this JogoDetalheServiceModel detalhe)
    {
        var jogo = detalhe.Jogo;

        return new ListarJogoResponseApiModel(
            jogo.Id,
            jogo.Nome,
            jogo.AnoLancamento,
            jogo.Preco,
            jogo.Estoque,
            jogo.Ativo,
            detalhe.NomePlataforma,
            detalhe.DescricaoCondicaoJogo,
            detalhe.Generos.Select(genero => genero.Nome).ToArray());
    }

    public static ListarPorIdJogoResponseApiModel ParaResponseDetalhe(
        this JogoDetalheServiceModel detalhe)
    {
        var jogo = detalhe.Jogo;

        return new ListarPorIdJogoResponseApiModel(
            jogo.Id,
            jogo.Nome,
            jogo.Descricao,
            jogo.AnoLancamento,
            jogo.Preco,
            jogo.Estoque,
            jogo.Ativo,
            jogo.IdPlataforma,
            detalhe.NomePlataforma,
            jogo.IdCondicaoJogo,
            detalhe.DescricaoCondicaoJogo,
            detalhe.Generos.Select(genero => genero.ParaResponse()).ToArray());
    }

    public static Jogo ParaModel(this InserirJogoRequestApiModel request)
    {
        return new Jogo
        {
            Nome = request.Nome,
            Descricao = request.Descricao,
            AnoLancamento = request.AnoLancamento,
            Preco = request.Preco,
            Estoque = request.Estoque,
            IdPlataforma = request.IdPlataforma,
            IdCondicaoJogo = request.IdCondicaoJogo
        };
    }

    public static Jogo ParaModel(this AlterarJogoRequestApiModel request, int id)
    {
        var jogo = ((InserirJogoRequestApiModel)request).ParaModel();
        jogo.Id = id;
        jogo.Ativo = request.Ativo == true;

        return jogo;
    }
}