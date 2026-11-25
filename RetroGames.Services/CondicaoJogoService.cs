using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public sealed class CondicaoJogoService : ICondicaoJogoService
{
    private readonly IRepositorySet _repositorios;

    public CondicaoJogoService(IRepositorySet repositorios)
    {
        _repositorios = repositorios;
    }

    public Task<IReadOnlyList<CondicaoJogo>> ListarAsync()
    {
        return _repositorios.CondicaoJogo.ListarAsync();
    }
}
