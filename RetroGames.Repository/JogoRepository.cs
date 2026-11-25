using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public sealed class JogoRepository : RepositoryBase, IJogoRepository
{
    private const string Colunas = """
        co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
        qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo
        """;

    public JogoRepository(RepositoryConfig config) : base(config)
    {
    }

    public async Task<(IReadOnlyList<Jogo> Itens, int Total)> ListarAsync(
        int pagina,
        int tamanhoPagina,
        string? busca = null,
        int? idPlataforma = null,
        IReadOnlyCollection<int>? idsJogos = null,
        bool incluirInativos = false)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pagina, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(tamanhoPagina, 1);

        if (idsJogos?.Count == 0)
        {
            return (Array.Empty<Jogo>(), 0);
        }

        var filtro = """
            WHERE (@incluirInativos = TRUE OR st_ativo = TRUE)
              AND (@busca IS NULL OR no_jogo LIKE CONCAT('%', @busca, '%'))
              AND (@idPlataforma IS NULL OR co_plataforma = @idPlataforma)
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, string.Empty);

        AdicionarParametro(comando, "@incluirInativos", incluirInativos);
        AdicionarParametro(comando, "@busca", busca);
        AdicionarParametro(comando, "@idPlataforma", idPlataforma);

        if (idsJogos is not null)
        {
            var nomesParametros = new List<string>(idsJogos.Count);
            foreach (var id in idsJogos)
            {
                var nome = $"@idJogo{nomesParametros.Count}";
                nomesParametros.Add(nome);
                AdicionarParametro(comando, nome, id);
            }

            filtro += $"\nAND co_seq_jogo IN ({string.Join(", ", nomesParametros)})";
        }

        comando.CommandText = $"SELECT COUNT(*) FROM tb_jogo {filtro}";
        var total = Convert.ToInt32(await comando.ExecuteScalarAsync());

        comando.CommandText = $"""
            SELECT {Colunas}
            FROM tb_jogo
            {filtro}
            ORDER BY no_jogo, co_seq_jogo
            LIMIT @limite OFFSET @deslocamento
            """;

        AdicionarParametro(comando, "@limite", tamanhoPagina);
        AdicionarParametro(comando, "@deslocamento", ((long)pagina - 1) * tamanhoPagina);

        await using var leitor = await comando.ExecuteReaderAsync();
        var jogos = new List<Jogo>();

        while (await leitor.ReadAsync())
        {
            jogos.Add(Mapear(leitor));
        }

        return (jogos, total);
    }

    public async Task<Jogo?> BuscarPorIdAsync(int id)
    {
        var sql = $"SELECT {Colunas} FROM tb_jogo WHERE co_seq_jogo = @id";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);

        await using var leitor = await comando.ExecuteReaderAsync();
        return await leitor.ReadAsync() ? Mapear(leitor) : null;
    }

    public async Task<int> InserirAsync(Jogo jogo)
    {
        const string sql = """
            INSERT INTO tb_jogo
                (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
                 st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
            VALUES
                (@nome, @descricao, @anoLancamento, @preco, @estoque,
                 @ativo, UTC_TIMESTAMP(), @idPlataforma, @idCondicaoJogo)
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarDadosDoJogo(comando, jogo);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);

        await ExecutarGravacaoAsync(comando);
        return checked((int)comando.LastInsertedId);
    }

    public async Task AlterarAsync(Jogo jogo)
    {
        const string sql = """
            UPDATE tb_jogo
            SET no_jogo = @nome,
                ds_jogo = @descricao,
                nu_ano_lancamento = @anoLancamento,
                vl_preco = @preco,
                qt_estoque = @estoque,
                co_plataforma = @idPlataforma,
                co_condicao_jogo = @idCondicaoJogo
            WHERE co_seq_jogo = @id
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarDadosDoJogo(comando, jogo);
        AdicionarParametro(comando, "@id", jogo.Id);

        await ExecutarGravacaoAsync(comando);
    }

    public async Task DesativarAsync(int id)
    {
        const string sql = """
            UPDATE tb_jogo
            SET st_ativo = FALSE
            WHERE co_seq_jogo = @id
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);

        await comando.ExecuteNonQueryAsync();
    }

    private static void AdicionarDadosDoJogo(MySqlCommand comando, Jogo jogo)
    {
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);
    }

    private static Jogo Mapear(MySqlDataReader leitor)
    {
        var colunaDescricao = leitor.GetOrdinal("ds_jogo");

        return new Jogo
        {
            Id = leitor.GetInt32("co_seq_jogo"),
            Nome = leitor.GetString("no_jogo"),
            Descricao = leitor.IsDBNull(colunaDescricao)
                ? null
                : leitor.GetString(colunaDescricao),
            AnoLancamento = leitor.GetInt32("nu_ano_lancamento"),
            Preco = leitor.GetDecimal("vl_preco"),
            Estoque = leitor.GetInt32("qt_estoque"),
            Ativo = leitor.GetBoolean("st_ativo"),
            DataCadastro = DateTime.SpecifyKind(
                leitor.GetDateTime("dt_cadastro"), DateTimeKind.Utc),
            IdPlataforma = leitor.GetInt32("co_plataforma"),
            IdCondicaoJogo = leitor.GetInt32("co_condicao_jogo")
        };
    }
}