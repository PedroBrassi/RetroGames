namespace RetroGames.WebApi.ApiModels;

public sealed record ListarJogoResponseApiModel(
    int Id,
    string Nome,
    int AnoLancamento,
    decimal Preco,
    int Estoque,
    bool Ativo,
    string NomePlataforma,
    string DescricaoCondicaoJogo,
    string[] Generos);