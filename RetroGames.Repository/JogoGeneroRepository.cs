using RetroGames.Model.Model;

namespace RetroGames.Repository;

public sealed class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig config) : base(config)
    {
    }

    public Task<IReadOnlyList<JogoGenero>> ListarTodosAsync()
    {
        return ListarAsync(null);
    }

    public Task<IReadOnlyList<JogoGenero>> ListarPorJogosAsync(
        IReadOnlyCollection<int> idsJogos)
    {
        return ListarAsync(idsJogos);
    }

    public async Task InserirAsync(int idJogo, IReadOnlyCollection<int> idsGeneros)
    {
        if (idsGeneros.Count == 0)
        {
            return;
        }

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, string.Empty);
        AdicionarParametro(comando, "@idJogo", idJogo);

        var valores = new List<string>(idsGeneros.Count);
        foreach (var idGenero in idsGeneros)
        {
            var parametro = $"@idGenero{valores.Count}";
            AdicionarParametro(comando, parametro, idGenero);
            valores.Add($"(@idJogo, {parametro})");
        }

        comando.CommandText = $"""
            INSERT INTO tb_jogo_genero (co_jogo, co_genero)
            VALUES {string.Join(", ", valores)}
            """;

        await ExecutarGravacaoAsync(comando);
    }

    public async Task RemoverPorJogoAsync(int idJogo)
    {
        const string sql = "DELETE FROM tb_jogo_genero WHERE co_jogo = @idJogo";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@idJogo", idJogo);

        await comando.ExecuteNonQueryAsync();
    }

    private async Task<IReadOnlyList<JogoGenero>> ListarAsync(
        IReadOnlyCollection<int>? idsJogos)
    {
        if (idsJogos?.Count == 0)
        {
            return Array.Empty<JogoGenero>();
        }

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, string.Empty);

        var filtro = string.Empty;
        if (idsJogos is not null)
        {
            var nomesParametros = new List<string>(idsJogos.Count);
            foreach (var id in idsJogos)
            {
                var nome = $"@idJogo{nomesParametros.Count}";
                nomesParametros.Add(nome);
                AdicionarParametro(comando, nome, id);
            }

            filtro = $"WHERE co_jogo IN ({string.Join(", ", nomesParametros)})";
        }

        comando.CommandText = $"""
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero
            {filtro}
            ORDER BY co_jogo, co_genero
            """;

        await using var leitor = await comando.ExecuteReaderAsync();
        var vinculos = new List<JogoGenero>();

        while (await leitor.ReadAsync())
        {
            vinculos.Add(new JogoGenero
            {
                IdJogo = leitor.GetInt32("co_jogo"),
                IdGenero = leitor.GetInt32("co_genero")
            });
        }

        return vinculos;
    }
}