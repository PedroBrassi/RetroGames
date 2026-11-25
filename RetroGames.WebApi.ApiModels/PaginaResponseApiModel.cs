namespace RetroGames.WebApi.ApiModels;

public sealed record PaginaResponseApiModel<T>(
    IReadOnlyList<T> Itens,
    int Pagina,
    int TamanhoPagina,
    int TotalItens,
    int TotalPaginas);