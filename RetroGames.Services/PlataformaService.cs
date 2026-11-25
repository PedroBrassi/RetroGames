using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public sealed class PlataformaService : IPlataformaService
{
    private readonly IRepositorySet _repositorios;

    public PlataformaService(IRepositorySet repositorios)
    {
        _repositorios = repositorios;
    }

    public Task<IReadOnlyList<Plataforma>> ListarAsync()
    {
        return _repositorios.Plataforma.ListarAsync();
    }

    public async Task<Plataforma> BuscarPorIdAsync(int id)
    {
        var plataforma = await _repositorios.Plataforma.BuscarPorIdAsync(id);
        return plataforma
            ?? throw new BusinessRuleException(BusinessErrors.PlataformaNaoEncontrada);
    }

    public async Task<Plataforma> InserirAsync(Plataforma plataforma)
    {
        plataforma.Nome = plataforma.Nome.Trim();
        await ConferirNomeDisponivelAsync(plataforma.Nome);

        try
        {
            plataforma.Id = await _repositorios.Plataforma.InserirAsync(plataforma);
            return plataforma;
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNomeDuplicado);
        }
    }

    public async Task AlterarAsync(Plataforma plataforma)
    {
        await BuscarPorIdAsync(plataforma.Id);
        plataforma.Nome = plataforma.Nome.Trim();
        await ConferirNomeDisponivelAsync(plataforma.Nome, plataforma.Id);

        try
        {
            await _repositorios.Plataforma.AlterarAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNomeDuplicado);
        }
    }

    public async Task ExcluirAsync(int id)
    {
        await BuscarPorIdAsync(id);

        if (await _repositorios.Plataforma.PossuiJogosAsync(id))
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaPossuiJogos);
        }

        await _repositorios.Plataforma.ExcluirAsync(id);
    }

    private async Task ConferirNomeDisponivelAsync(string nome, int? idIgnorar = null)
    {
        if (await _repositorios.Plataforma.ExisteComNomeAsync(nome, idIgnorar))
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNomeDuplicado);
        }
    }
}
