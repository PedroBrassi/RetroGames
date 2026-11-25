using System.Net;

namespace RetroGames.Services;

public static class BusinessErrors
{
    public static BusinessError PlataformaNaoEncontrada { get; } =
        new(100, "Plataforma não encontrada.", HttpStatusCode.NotFound);

    public static BusinessError PlataformaNomeDuplicado { get; } =
        new(101, "Já existe uma plataforma com este nome.", HttpStatusCode.Conflict);

    public static BusinessError PlataformaPossuiJogos { get; } =
        new(102, "Não é possível excluir: existem jogos desta plataforma.", HttpStatusCode.Conflict);

    public static BusinessError JogoNaoEncontrado { get; } =
        new(103, "Jogo não encontrado.", HttpStatusCode.NotFound);

    public static BusinessError CondicaoNaoEncontrada { get; } =
        new(104, "A condição informada não existe.", HttpStatusCode.NotFound);

    public static BusinessError GeneroNaoEncontrado { get; } =
        new(105, "Um ou mais gêneros informados não existem.", HttpStatusCode.NotFound);
}
