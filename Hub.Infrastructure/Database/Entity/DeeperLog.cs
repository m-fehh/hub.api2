namespace Hub.Infrastructure.Database.Entity
{
    /// <summary>
    /// Indica se a gravação de log entrará na propriedade em busca de alterações no item (a propriedade pode ser um objeto complexo ou uma lista)
    /// </summary>
    public class DeeperLog : Attribute { }
}
