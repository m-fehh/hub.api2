using System.Collections;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Hub.Infrastructure.Architecture;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Entity;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Extensions;
using Hub.Infrastructure.Nominator.Interfaces;
using Hub.Infrastructure.Nominator;

namespace FMK.Core.Nominator
{
    public class NominatorManager : INominatorManager
    {
        private readonly DbContext _dbContext;

        // Construtor para injetar dependência do DbContext
        public NominatorManager(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Método principal para obter o nome do objeto
        public string GetName(object obj)
        {
            if (obj == null) return string.Empty;

            var namesProperties = GetMainNominateProperties(obj);

            string name = "";

            if (namesProperties.Any())
            {
                foreach (var prop in namesProperties.OrderBy(p => ((MainNominate)p.GetCustomAttributes(typeof(MainNominate), true).FirstOrDefault()).Order))
                {
                    name += GetPropertyDescritor(prop, obj) + " - ";
                }

                name = name.Substring(0, name.Length - 3); // Remove a última " - "
            }
            else
            {
                // Tentativa de encontrar uma boa propriedade para representar o nome
                name = TryFindKeyProperty(obj);
            }

            return name;
        }

        // Método para obter as propriedades com o atributo MainNominate
        private IEnumerable<PropertyInfo> GetMainNominateProperties(object obj)
        {
            return obj.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(MainNominate), true).Any());
        }

        // Método para tentar encontrar a propriedade mais adequada (como Name, Description, etc)
        private string TryFindKeyProperty(object obj)
        {
            var keyWords = new[] { "Name", "Description", "Code", "Key" };
            var ignoredKeywords = new[] { "ExternalCode" };

            foreach (var key in keyWords)
            {
                var prop = obj.GetType().GetProperties().FirstOrDefault(p => p.Name.Equals(key, StringComparison.OrdinalIgnoreCase) && !ignoredKeywords.Contains(p.Name));

                if (prop != null)
                {
                    return GetPropertyDescritor(prop, obj);
                }
            }

            // Tentativa de encontrar uma propriedade que começa com as palavras-chave
            foreach (var key in keyWords)
            {
                var prop = obj.GetType().GetProperties().FirstOrDefault(p => p.Name.StartsWith(key, StringComparison.OrdinalIgnoreCase) && !ignoredKeywords.Contains(p.Name));

                if (prop != null)
                {
                    return GetPropertyDescritor(prop, obj);
                }
            }

            // Tentativa de encontrar uma propriedade que contenha as palavras-chave
            foreach (var key in keyWords)
            {
                var prop = obj.GetType().GetProperties().FirstOrDefault(p => p.Name.Contains(key, StringComparison.OrdinalIgnoreCase) && !ignoredKeywords.Contains(p.Name));

                if (prop != null)
                {
                    return GetPropertyDescritor(prop, obj);
                }
            }

            // Se não encontrar nada, tenta usar a propriedade "Id"
            var idProp = obj.GetType().GetProperty("Id");
            if (idProp != null)
            {
                return GetPropertyDescritor(idProp, obj);
            }

            return obj.ToString();
        }

        // Método para obter o descritor da propriedade
        public string GetPropertyDescritor(PropertyInfo prop, object obj, bool refreshIfNull = true)
        {
            if (prop.GetMethod == null || obj == null) return string.Empty;

            var value = prop.GetValue(obj);

            if (value == null && refreshIfNull && obj is IBaseEntity baseEntity)
            {
                value = prop.GetValue(obj);
            }

            if (value == null) return string.Empty;

            // Verifica se o tipo é primitivo e formata de acordo
            if (PrimitiveTypes.Test(prop.PropertyType))
            {
                return FormatPrimitiveValue(prop.PropertyType, value);
            }

            // Se for uma coleção, retorna o número de itens
            if (IsCollection(prop.PropertyType))
            {
                return GetCollectionCount(value);
            }

            // Caso contrário, recursivamente chama GetName para o valor
            return GetName(value);
        }

        // Método para formatar tipos primitivos como DateTime, Decimal, Boolean, etc
        private string FormatPrimitiveValue(Type type, object value)
        {
            if (type == typeof(DateTime) || type == typeof(DateTime?))
                return ((DateTime?)value)?.ToString("dd/MM/yyyy HH:mm:ss");

            if (type == typeof(Decimal) || type == typeof(Decimal?))
                return ((Decimal?)value)?.ToString("0.00");

            if (type == typeof(Double) || type == typeof(Double?))
                return ((Double?)value)?.ToString("0.00");

            if (type == typeof(Boolean) || type == typeof(Boolean?))
                return ((Boolean?)value).GetValueOrDefault() ? "verdadeiro" : "falso";

            if (type.IsEnum || (Nullable.GetUnderlyingType(type)?.IsEnum ?? false))
                return value.ToString();

            return value.ToString();
        }

        // Método para verificar se é uma coleção
        private bool IsCollection(Type type)
        {
            return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        // Método para contar os itens de uma coleção
        private string GetCollectionCount(object value)
        {
            var count = (value as IEnumerable).Cast<IBaseEntity>().Count(o => !(o is IListItemEntity item && item.DeleteFromList));

            return $"{count} {(count == 1 ? "item" : "itens")}";
        }
    }
}
