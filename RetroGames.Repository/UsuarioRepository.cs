using RetroGames.Model.Constantes;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public sealed class UsuarioRepository : RepositoryBase, IUsuarioRepository
{
    private const string Colunas = """
        co_seq_usuario, no_usuario, ds_email, ds_senha_hash,
        st_ativo, dt_cadastro, co_perfil
        """;

    public UsuarioRepository(RepositoryConfig config) : base(config)
    {
    }

    public Task<Usuario?> BuscarPorEmailAsync(string email)
    {
        var sql = $"SELECT {Colunas} FROM tb_usuario WHERE ds_email = @email";
        return BuscarAsync(sql, "@email", email);
    }

    public Task<Usuario?> BuscarPorIdAsync(int id)
    {
        var sql = $"SELECT {Colunas} FROM tb_usuario WHERE co_seq_usuario = @id";
        return BuscarAsync(sql, "@id", id);
    }

    public async Task<bool> ExisteAdministradorAsync()
    {
        const string sql = """
            SELECT 1 FROM tb_usuario
            WHERE co_perfil = @idPerfil
            LIMIT 1
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idPerfil", PerfilIds.Administrador);

        return await comando.ExecuteScalarAsync() is not null;
    }

    public async Task<int> InserirAsync(Usuario usuario)
    {
        const string sql = """
            INSERT INTO tb_usuario
                (no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil)
            VALUES
                (@nome, @email, @senhaHash, @ativo, UTC_TIMESTAMP(), @idPerfil)
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", usuario.Nome);
        AdicionarParametro(comando, "@email", usuario.Email);
        AdicionarParametro(comando, "@senhaHash", usuario.SenhaHash);
        AdicionarParametro(comando, "@ativo", usuario.Ativo);
        AdicionarParametro(comando, "@idPerfil", usuario.IdPerfil);

        await ExecutarGravacaoAsync(comando);
        return checked((int)comando.LastInsertedId);
    }

    private async Task<Usuario?> BuscarAsync(string sql, string parametro, object valor)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, parametro, valor);

        await using var leitor = await comando.ExecuteReaderAsync();
        if (!await leitor.ReadAsync())
        {
            return null;
        }

        return new Usuario
        {
            Id = leitor.GetInt32("co_seq_usuario"),
            Nome = leitor.GetString("no_usuario"),
            Email = leitor.GetString("ds_email"),
            SenhaHash = leitor.GetString("ds_senha_hash"),
            Ativo = leitor.GetBoolean("st_ativo"),
            DataCadastro = DateTime.SpecifyKind(
                leitor.GetDateTime("dt_cadastro"), DateTimeKind.Utc),
            IdPerfil = leitor.GetInt32("co_perfil")
        };
    }
}