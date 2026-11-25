namespace RetroGames.WebApi.ApiModels;

public sealed record ListarPlataformaResponseApiModel(
    int Id,
    string Nome,
    string Fabricante,
    int AnoLancamento);