namespace Hub.Infrastructure.Database.Entity.Interfaces
{
    /// <summary>
    /// As entidades que são controladas por outra entidade, devem ser marcadas por essa interface para serem reconhecidas como itens de lista
    /// </summary>
    public interface IListItemEntity
    {
        bool DeleteFromList { get; set; }
    }
}
