namespace RetroGames.Services;

public sealed class BusinessRuleException : Exception
{
    public BusinessRuleException(BusinessError erro) : base(erro.Mensagem)
    {
        Erro = erro;
    }

    public BusinessError Erro { get; }
}
