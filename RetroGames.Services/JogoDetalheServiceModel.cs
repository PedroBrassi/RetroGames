using RetroGames.Model.Model;

namespace RetroGames.Services;

public sealed record JogoDetalheServiceModel(
    Jogo Jogo,
    string NomePlataforma,
    string DescricaoCondicaoJogo,
    IReadOnlyList<Genero> Generos);