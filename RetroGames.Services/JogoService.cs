using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public sealed class JogoService : IJogoService
{
    private readonly IRepositorySet _repositorios;

    public JogoService(IRepositorySet repositorios)
    {
        _repositorios = repositorios;
    }

    public async Task<(IReadOnlyList<JogoDetalheServiceModel> Itens, int Total)> ListarAsync(
        int pagina,
        int tamanhoPagina,
        string? busca = null,
        int? idPlataforma = null,
        int? idGenero = null,
        bool incluirInativos = false)
    {
        int[]? idsJogos = null;

        if (idGenero is int genero)
        {
            var vinculos = await _repositorios.JogoGenero.ListarTodosAsync();
            idsJogos = vinculos
                .Where(vinculo => vinculo.IdGenero == genero)
                .Select(vinculo => vinculo.IdJogo)
                .ToArray();
        }

        var (jogos, total) = await _repositorios.Jogo.ListarAsync(
            pagina, tamanhoPagina, busca, idPlataforma, idsJogos, incluirInativos);

        return (await MontarDetalhesAsync(jogos), total);
    }

    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var jogo = await ObterJogoAsync(id);
        var detalhes = await MontarDetalhesAsync(new[] { jogo });

        return detalhes[0];
    }

    public async Task<JogoDetalheServiceModel> InserirAsync(
        Jogo jogo, IReadOnlyCollection<int> idsGeneros)
    {
        var generos = idsGeneros.Distinct().ToArray();
        await ConferirReferenciasAsync(jogo, generos);

        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = true;

        using (var transacao = CriarTransacao())
        {
            jogo.Id = await _repositorios.Jogo.InserirAsync(jogo);
            await _repositorios.JogoGenero.InserirAsync(jogo.Id, generos);
            transacao.Complete();
        }

        return await BuscarPorIdAsync(jogo.Id);
    }

    public async Task AlterarAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros)
    {
        await ObterJogoAsync(jogo.Id);

        var generos = idsGeneros.Distinct().ToArray();
        await ConferirReferenciasAsync(jogo, generos);
        jogo.Nome = jogo.Nome.Trim();

        using (var transacao = CriarTransacao())
        {
            await _repositorios.Jogo.AlterarAsync(jogo);
            await _repositorios.JogoGenero.RemoverPorJogoAsync(jogo.Id);
            await _repositorios.JogoGenero.InserirAsync(jogo.Id, generos);
            transacao.Complete();
        }
    }

    public async Task DesativarAsync(int id)
    {
        await ObterJogoAsync(id);
        await _repositorios.Jogo.DesativarAsync(id);
    }

    private async Task<Jogo> ObterJogoAsync(int id)
    {
        var jogo = await _repositorios.Jogo.BuscarPorIdAsync(id);
        return jogo
            ?? throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
    }

    private async Task ConferirReferenciasAsync(
        Jogo jogo, IReadOnlyCollection<int> idsGeneros)
    {
        var plataforma = await _repositorios.Plataforma.BuscarPorIdAsync(jogo.IdPlataforma);
        if (plataforma is null)
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNaoEncontrada);
        }

        var condicoes = await _repositorios.CondicaoJogo.ListarAsync();
        if (!condicoes.Any(condicao => condicao.Id == jogo.IdCondicaoJogo))
        {
            throw new BusinessRuleException(BusinessErrors.CondicaoNaoEncontrada);
        }

        var generos = await _repositorios.Genero.ListarAsync();
        var idsExistentes = generos.Select(genero => genero.Id).ToHashSet();

        if (idsGeneros.Count == 0 || idsGeneros.Any(id => !idsExistentes.Contains(id)))
        {
            throw new BusinessRuleException(BusinessErrors.GeneroNaoEncontrado);
        }
    }

    private async Task<IReadOnlyList<JogoDetalheServiceModel>> MontarDetalhesAsync(
        IReadOnlyList<Jogo> jogos)
    {
        if (jogos.Count == 0)
        {
            return Array.Empty<JogoDetalheServiceModel>();
        }

        var plataformas = (await _repositorios.Plataforma.ListarAsync())
            .ToDictionary(plataforma => plataforma.Id, plataforma => plataforma.Nome);

        var condicoes = (await _repositorios.CondicaoJogo.ListarAsync())
            .ToDictionary(condicao => condicao.Id, condicao => condicao.Descricao);

        var generos = (await _repositorios.Genero.ListarAsync())
            .ToDictionary(genero => genero.Id);

        var idsJogos = jogos.Select(jogo => jogo.Id).ToArray();
        var vinculos = await _repositorios.JogoGenero.ListarPorJogosAsync(idsJogos);
        var generosPorJogo = vinculos.ToLookup(vinculo => vinculo.IdJogo, vinculo => vinculo.IdGenero);

        return jogos.Select(jogo => new JogoDetalheServiceModel(
            jogo,
            plataformas[jogo.IdPlataforma],
            condicoes[jogo.IdCondicaoJogo],
            generosPorJogo[jogo.Id]
                .Select(id => generos[id])
                .OrderBy(genero => genero.Nome)
                .ToArray()))
            .ToArray();
    }

    private static TransactionScope CriarTransacao()
    {
        var opcoes = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        return new TransactionScope(
            TransactionScopeOption.Required,
            opcoes,
            TransactionScopeAsyncFlowOption.Enabled);
    }
}