using RetroGames.Model.Model;

namespace RetroGames.Services;

public interface IJogoService
{
    Task<(IReadOnlyList<JogoDetalheServiceModel> Itens, int Total)> ListarAsync(
        int pagina,
        int tamanhoPagina,
        string? busca = null,
        int? idPlataforma = null,
        int? idGenero = null,
        bool incluirInativos = false);

    Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id);
    Task<JogoDetalheServiceModel> InserirAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros);
    Task AlterarAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros);
    Task DesativarAsync(int id);
}