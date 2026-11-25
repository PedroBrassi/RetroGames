namespace RetroGames.Repository;

public sealed class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException(Exception causa)
        : base("Já existe um registro com o mesmo valor em uma coluna única.", causa)
    {
    }
}
