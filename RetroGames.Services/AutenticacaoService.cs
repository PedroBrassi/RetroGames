using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public sealed class AutenticacaoService : IAutenticacaoService
{
    private readonly IRepositorySet _repositorios;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public AutenticacaoService(
        IRepositorySet repositorios,
        IPasswordHasher<Usuario> passwordHasher)
    {
        _repositorios = repositorios;
        _passwordHasher = passwordHasher;
    }

    public async Task<Usuario?> AutenticarAsync(string email, string senha)
    {
        var usuario = await _repositorios.Usuario.BuscarPorEmailAsync(email.Trim());

        if (usuario is null || !usuario.Ativo)
        {
            return null;
        }

        var resultado = _passwordHasher.VerifyHashedPassword(
            usuario, usuario.SenhaHash, senha);

        return resultado == PasswordVerificationResult.Failed ? null : usuario;
    }

    public async Task GarantirAdministradorInicialAsync(string? email, string? senha)
    {
        if (await _repositorios.Usuario.ExisteAdministradorAsync())
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(senha) ||
            senha == "SUA_SENHA")
        {
            throw new InvalidOperationException(
                "Configure AdminInicial__Email e AdminInicial__Senha no .env.");
        }

        var administrador = new Usuario
        {
            Nome = Perfis.Administrador,
            Email = email.Trim(),
            Ativo = true,
            IdPerfil = PerfilIds.Administrador
        };

        administrador.SenhaHash = _passwordHasher.HashPassword(administrador, senha);

        try
        {
            await _repositorios.Usuario.InserirAsync(administrador);
        }
        catch (RegistroDuplicadoException)
        {
            if (!await _repositorios.Usuario.ExisteAdministradorAsync())
            {
                throw new InvalidOperationException(
                    "O e-mail configurado para o administrador inicial já está em uso.");
            }
        }
    }
}