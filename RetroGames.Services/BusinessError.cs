using System.Net;

namespace RetroGames.Services;

public sealed record BusinessError(int Codigo, string Mensagem, HttpStatusCode Status);
