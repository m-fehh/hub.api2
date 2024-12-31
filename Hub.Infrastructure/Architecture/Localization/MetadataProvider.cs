using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace Hub.Infrastructure.Architecture.Localization
{
    public class MetadataProvider : IDisplayMetadataProvider
    {
        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            var propertyAttributes = context.Attributes;

            var modelType = context.Key.ModelType;

            var propertyName = context.Key.Name;

            string sKey = string.Empty;

            foreach (var attr in propertyAttributes)
            {
                if (attr != null)
                {
                    if (attr is DisplayAttribute)
                    {
                        context.DisplayMetadata.DisplayName = () =>
                        {
                            var sKey = ((DisplayAttribute)attr).Name;

                            if (!string.IsNullOrEmpty(sKey))
                            {
                                return Get(sKey, GenerateUniqueGuid(modelType, (DisplayAttribute)attr, propertyName));
                            }
                            return " ";
                        };
                    }
                    else if (attr is ValidationAttribute)
                    {
                        sKey = ((ValidationAttribute)attr).ErrorMessage;

                        if (!string.IsNullOrEmpty(sKey))
                        {
                            ((ValidationAttribute)attr).ErrorMessage = Get(sKey, GenerateUniqueGuid(modelType, (ValidationAttribute)attr, propertyName));
                        }
                        //else if (attr is RequiredAttribute || attr is PasswordValidationAttribute)
                        else if (attr is RequiredAttribute)
                        {
                            ((ValidationAttribute)attr).ErrorMessage = Get("generic_required_message", GenerateUniqueGuid(modelType, (ValidationAttribute)attr, propertyName));
                        }
                        else if (attr is MaxLengthAttribute)
                        {
                            ((ValidationAttribute)attr).ErrorMessage = Get("generic_maxlength_message", GenerateUniqueGuid(modelType, (ValidationAttribute)attr, propertyName));
                        }
                        else if (attr is MinLengthAttribute)
                        {
                            ((ValidationAttribute)attr).ErrorMessage = Get("generic_minlength_message", GenerateUniqueGuid(modelType, (ValidationAttribute)attr, propertyName));
                        }
                        else if (attr is RangeAttribute)
                        {
                            ((ValidationAttribute)attr).ErrorMessage = Get("generic_range_message", GenerateUniqueGuid(modelType, (ValidationAttribute)attr, propertyName));
                        }
                        else if (attr is StringLengthAttribute)
                        {
                            ((ValidationAttribute)attr).ErrorMessage = Get("generic_maxlength_message", GenerateUniqueGuid(modelType, (ValidationAttribute)attr, propertyName));
                        }
                    }
                }
            }
        }

        string GenerateUniqueGuid(Type modelType, Attribute attr, string propertyName)
        {
            return
                modelType.GetHashCode().ToString() +
                attr.GetHashCode().ToString() +
                propertyName;
        }

        /// <summary>
        /// Para otimizar a performance, primeiro é feita uma checagem na coleção específica para atributos, se não encontrado, então parte para o provedor de recursos (TextResource).
        /// </summary>
        /// <param name="key"></param>
        /// <param name="attributeGuid">cada atributo possui um único GUID gerado pelo sistema. Ele será usado para controlar nossa coleção</param>
        /// <returns></returns>
        string Get(string key, string attributeGuid)
        {
            if (string.IsNullOrEmpty(key)) return " ";

            var culture = Thread.CurrentThread.CurrentCulture.Name;

            if (Singleton<LocalizationAttributeCollection>.Instance.Itens.ContainsKey(attributeGuid))
            {
                var item = Singleton<LocalizationAttributeCollection>.Instance.Itens[attributeGuid];

                if (item.CultureTranslation.ContainsKey(culture))
                {
                    return item.CultureTranslation[culture] ?? " ";
                }
                else
                {
                    var textResource = Engine.Get(item.Key);

                    item.CultureTranslation.AddOrUpdate(culture, textResource,
                        (KeyNotFoundException, existingVal) =>
                        {
                            return existingVal;
                        });

                    return textResource ?? " ";
                }
            }
            else
            {
                var textResource = Engine.Get(key);

                var item = new LocalizationAttributeItem();

                item.Key = key;

                item.CultureTranslation.AddOrUpdate(culture, textResource,
                            (KeyNotFoundException, existingVal) =>
                            {
                                return existingVal;
                            });

                Singleton<LocalizationAttributeCollection>.Instance.Itens.AddOrUpdate(attributeGuid, item,
                    (KeyNotFoundException, existingVal) =>
                    {
                        return existingVal;
                    });

                return textResource ?? " ";
            }
        }

    }

    internal class LocalizationAttributeItem
    {
        public LocalizationAttributeItem()
        {
            CultureTranslation = new ConcurrentDictionary<string, string>();
        }

        public string Key { get; set; }
        public ConcurrentDictionary<string, string> CultureTranslation { get; set; }
    }

    /// <summary>
    /// Coleção de traduções dos atributos de dataannotations. 
    /// Essa classe é usada para otimizar a performace na hora de localizar as chaves dos atributos e evitar multiplas consultas ao provedor de recursos (TextResource)
    /// </summary>
    internal class LocalizationAttributeCollection
    {
        public LocalizationAttributeCollection()
        {
            Itens = new ConcurrentDictionary<string, LocalizationAttributeItem>();
        }

        public ConcurrentDictionary<string, LocalizationAttributeItem> Itens { get; set; }
    }

    public static class LocalizationAttributeBootstrap
    {
        public static void Initialize()
        {
            Singleton<LocalizationAttributeCollection>.Instance = new LocalizationAttributeCollection();
        }
    }
}
