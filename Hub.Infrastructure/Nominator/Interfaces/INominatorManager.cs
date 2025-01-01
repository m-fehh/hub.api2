using System.Reflection;

namespace Hub.Infrastructure.Nominator.Interfaces
{
    public interface INominatorManager
    {
        /// <summary>
        /// as classes que implementarem esse método devem fornecer um nome para o objeto passado, normalmente a estratégia é usar algum marcador de propriedade para saber quais serão usadas para montar o nome.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string GetName(object obj);

        /// <summary>
        /// as classes que implementarem esse método devem conseguir retornar o valor da propriedade para o objeto do parametro em formato de texto
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="obj"></param>
        /// <param name="refreshIfNull"></param>
        /// <returns></returns>
        string GetPropertyDescritor(PropertyInfo prop, object obj, bool refreshIfNull = true);
    }
}
