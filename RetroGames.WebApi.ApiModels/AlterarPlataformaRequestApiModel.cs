namespace RetroGames.WebApi.ApiModels;

public sealed class AlterarPlataformaRequestApiModel
{
    public string Nome { get; init; } = string.Empty;
    public string Fabricante { get; init; } = string.Empty;
    public int AnoLancamento { get; init; }
}