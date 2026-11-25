using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public sealed class GeneroService : IGeneroService
{
    private readonly IRepositorySet _repositorios;

    public GeneroService(IRepositorySet repositorios)
    {
        _repositorios = repositorios;
    }

    public Task<IReadOnlyList<Genero>> ListarAsync()
    {
        return _repositorios.Genero.ListarAsync();
    }
}
