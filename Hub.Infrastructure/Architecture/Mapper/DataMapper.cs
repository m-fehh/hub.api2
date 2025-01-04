//#region ANTES - REFACTOR
//using Hub.Infrastructure.Architecture.Mapper.Interfaces;
//using Hub.Infrastructure.Database.Entity.Interfaces;
//using Hub.Infrastructure.Database.Interfaces;
//using Hub.Infrastructure.Extensions;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Hub.Infrastructure.Architecture.Mapper
//{
//    public class DataMapper<TEntity, TModel> : IDataMapper<TEntity, TModel> where TEntity : IBaseEntity where TModel : ICrudModel
//    {
//        private readonly IRepository<TEntity> repository;

//        public DataMapper(IRepository<TEntity> repository)
//        {
//            this.repository = repository;
//        }

//        /// <summary>
//        /// Constrói o modelo a partir de uma entidade (substitui o AutoMapper)
//        /// Este mapeamento ignora as listas
//        /// </summary>
//        /// <param name="entity"></param>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        public TModel MapModel(TEntity entity, TModel model = default(TModel))
//        {
//            if (model == null)
//            {
//                model = Activator.CreateInstance<TModel>();
//            }

//            foreach (var mPi in typeof(TModel).GetProperties().Where(m => m.SetMethod != null && (m.PropertyType.IsGenericType == false || (m.PropertyType.IsGenericType && m.PropertyType.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(System.Collections.IList)) == false))))
//            {
//                var ePi = typeof(TEntity).GetProperty(mPi.Name);

//                if (ePi != null)
//                {
//                    if (ePi.GetMethod != null && ePi.CustomAttributes.Any(c => c.AttributeType == typeof(NHibernate.Mapping.Attributes.OneToManyAttribute)) == false)
//                    {
//                        mPi.SetValue(model, ePi.GetValue(entity));
//                    }
//                }
//                else if (mPi.Name.EndsWith("_Id"))
//                {
//                    ePi = typeof(TEntity).GetProperty(mPi.Name.Replace("_Id", ""));

//                    if (ePi != null && ePi.GetMethod != null && typeof(IBaseEntity).IsAssignableFrom(ePi.PropertyType))
//                    {
//                        var reference = (IBaseEntity)ePi.GetValue(entity);

//                        if (reference != null)
//                        {
//                            mPi.SetValue(model, reference.Id);
//                        }
//                        else
//                        {
//                            mPi.SetValue(model, null);
//                        }
//                    }
//                }
//            }

//            return model;
//        }

//        /// <summary>
//        /// Constrói a entidade a partir de um modelo (substitui o AutoMapper)
//        /// Este mapeamento ignora as listas
//        /// </summary>
//        /// <param name="model"></param>
//        /// <param name="entity"></param>
//        /// <returns></returns>
//        public TEntity MapEntity(TModel model, TEntity entity = default(TEntity))
//        {
//            if (entity == null)
//            {
//                entity = Activator.CreateInstance<TEntity>();
//            }

//            foreach (var mPi in typeof(TModel).GetProperties().Where(m =>
//                m.GetMethod != null &&
//                (m.PropertyType.IsGenericType == false || (m.PropertyType.IsGenericType &&
//                 m.PropertyType.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(System.Collections.IList)) == false))))
//            {
//                var ePi = typeof(TEntity).GetProperty(mPi.Name);

//                if (ePi != null)
//                {
//                    if (ePi.SetMethod != null && ePi.CustomAttributes.Any(c => c.AttributeType == typeof(NHibernate.Mapping.Attributes.OneToManyAttribute)) == false)
//                    {
//                        ePi.SetValue(entity, mPi.GetValue(model));
//                    }
//                }
//                else if (mPi.Name.EndsWith("_Id"))
//                {
//                    ePi = typeof(TEntity).GetProperty(mPi.Name.Replace("_Id", ""));

//                    if (ePi != null && ePi.SetMethod != null && typeof(IBaseEntity).IsAssignableFrom(ePi.PropertyType))
//                    {
//                        var referenceId = (long?)mPi.GetValue(model);

//                        if (referenceId != null)
//                        {
//                            var reference = (IBaseEntity)Activator.CreateInstance(ePi.PropertyType);

//                            reference.Id = referenceId.Value;

//                            ePi.SetValue(entity, reference);
//                        }
//                        else
//                        {
//                            ePi.SetValue(entity, null);
//                        }
//                    }
//                }
//            }

//            return entity;
//        }

//        /// <summary>
//        /// Gera o modelo a partir da entidade
//        /// </summary>
//        /// <param name="entity"></param>
//        /// <returns></returns>
//        public TModel BuildModel(TEntity entity)
//        {
//            TModel model = Activator.CreateInstance<TModel>();

//            model = MapModel(entity, model);

//            if (entity != null && entity.Id != 0)
//            {
//                model.SerializedOldValue = model.SerializeToJSON();
//            }

//            return model;
//        }


//        /// <summary>
//        /// Gera a entidade a partir do modelo (
//        /// </summary>
//        /// <param name="model"></param>
//        /// <returns></returns>
//        public TEntity BuildEntity(TModel model, long? id = null)
//        {
//            if (model == null) return default(TEntity);

//            TEntity entity = default(TEntity);

//            TModel modelFromEntity = Activator.CreateInstance<TModel>();

//            if (model.Id != null || id != null)
//            {
//                if (id == null) id = model.Id.Value;

//                entity = repository.GetById(id.Value);

//                modelFromEntity = MapModel(entity, modelFromEntity);
//            }

//            TModel oldModel = default(TModel);

//            //model antes de ser modificado, serve de referencia para alterar apenas as propriedades alteradas
//            if (model.SerializedOldValue != null)
//            {
//                oldModel = JsonConvert.DeserializeObject<TModel>(model.SerializedOldValue);
//            }

//            if (oldModel != null)
//            {
//                foreach (var pi in model.GetType().GetProperties().Where(m => m.SetMethod != null && !typeof(ICollection<>).IsAssignableFrom(m.PropertyType)))
//                {
//                    //neste ponto é feita uma procurar por todas as propriedades não alteradas do modelo
//                    if ((pi.GetValue(model) == pi.GetValue(oldModel)) || (pi.GetValue(model) != null && pi.GetValue(model).Equals(pi.GetValue(oldModel))))
//                    {
//                        //para cada propriedade, copia o valor que veio do banco, assim não haverá update
//                        pi.SetValue(model, pi.GetValue(modelFromEntity));
//                    }
//                }
//            }

//            //setar a entidade com os dados da model
//            entity = MapEntity(model, entity);

//            return entity;
//        }
//    }
//}

////#endregion
///

using Hub.Infrastructure.Architecture.Mapper.Interfaces;
using Hub.Infrastructure.Database.Entity.Interfaces;
using Hub.Infrastructure.Database.Interfaces;
using Hub.Infrastructure.Extensions;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;

namespace Hub.Infrastructure.Architecture.Mapper
{
    public class DataMapper<TEntity, TModel> : IDataMapper<TEntity, TModel> where TEntity : IBaseEntity where TModel : IModelEntity
    {
        private readonly IRepository<TEntity> repository;

        /// <summary>
        /// Construtor que recebe o repositório da entidade.
        /// </summary>
        /// <param name="repository">Repositório da entidade</param>
        public DataMapper(IRepository<TEntity> repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Mapeia a entidade para o modelo, ignorando listas e propriedades de relacionamento.
        /// </summary>
        /// <param name="entity">Entidade a ser mapeada</param>
        /// <param name="model">Modelo a ser preenchido</param>
        /// <returns>Modelo preenchido a partir da entidade</returns>
        public TModel MapModel(TEntity entity, TModel model = default)
        {
            model ??= Activator.CreateInstance<TModel>();

            var entityProperties = typeof(TEntity).GetProperties();
            var modelProperties = typeof(TModel).GetProperties().Where(m => m.SetMethod != null && !IsNavigationProperty(m)); // Evita propriedades de navegação

            foreach (var modelProp in modelProperties)
            {
                var entityProp = entityProperties.FirstOrDefault(e => e.Name == modelProp.Name);

                if (entityProp != null)
                {
                    modelProp.SetValue(model, entityProp.GetValue(entity));
                }
                else if (modelProp.Name.EndsWith("_Id"))
                {
                    MapEntityReference(entity, modelProp, entityProp, model);
                }
            }

            return model;
        }

        /// <summary>
        /// Mapeia o modelo para a entidade, ignorando listas e propriedades de relacionamento.
        /// </summary>
        /// <param name="model">Modelo a ser mapeado</param>
        /// <param name="entity">Entidade a ser preenchida</param>
        /// <returns>Entidade preenchida a partir do modelo</returns>
        public TEntity MapEntity(TModel model, TEntity entity = default)
        {
            entity ??= Activator.CreateInstance<TEntity>();

            var modelProperties = typeof(TModel).GetProperties();
            var entityProperties = typeof(TEntity).GetProperties().Where(e => e.SetMethod != null && !IsNavigationProperty(e)); // Evita propriedades de navegação

            foreach (var modelProp in modelProperties)
            {
                var entityProp = entityProperties.FirstOrDefault(e => e.Name == modelProp.Name);

                if (entityProp != null)
                {
                    entityProp.SetValue(entity, modelProp.GetValue(model));
                }
                else if (modelProp.Name.EndsWith("_Id"))
                {
                    MapEntityReferenceToId(model, modelProp, entityProp, entity);
                }
            }

            return entity;
        }

        /// <summary>
        /// Cria e mapeia o modelo a partir da entidade, incluindo a serialização do valor anterior.
        /// </summary>
        /// <param name="entity">Entidade a ser mapeada</param>
        /// <returns>Modelo preenchido a partir da entidade</returns>
        public TModel BuildModel(TEntity entity)
        {
            var model = MapModel(entity, Activator.CreateInstance<TModel>());

            if (entity?.Id != 0)
            {
                model.SerializedOldValue = model.SerializeToJSON(); // Serializa o valor anterior da entidade
            }

            return model;
        }

        /// <summary>
        /// Cria e mapeia a entidade a partir do modelo, realizando comparação e atualização das propriedades.
        /// </summary>
        /// <param name="model">Modelo a ser mapeado</param>
        /// <param name="id">ID para busca da entidade no repositório</param>
        /// <returns>Entidade preenchida a partir do modelo</returns>
        public TEntity BuildEntity(TModel model, long? id = null)
        {
            if (model == null) return default;

            TEntity entity = default;
            TModel modelFromEntity = Activator.CreateInstance<TModel>();

            if (model.Id != null || id != null)
            {
                id ??= model.Id;
                entity = repository.GetById(id.Value);
                modelFromEntity = MapModel(entity, modelFromEntity);
            }

            TModel oldModel = DeserializeOldModel(model.SerializedOldValue);

            if (oldModel != null)
            {
                SyncUnchangedProperties(model, oldModel, modelFromEntity);
            }

            entity = MapEntity(model, entity);

            return entity;
        }

        #region PRIVATE METHODS   

        /// <summary>
        /// Verifica se a propriedade é uma propriedade de navegação (coleção de entidade).
        /// </summary>
        /// <param name="property">Propriedade a ser verificada</param>
        /// <returns>True se for uma propriedade de navegação, caso contrário False</returns>
        private static bool IsNavigationProperty(PropertyInfo property) => property.PropertyType.IsGenericType && typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string);  // Evita que propriedades de tipo string sejam interpretadas como coleções

        /// <summary>
        /// Mapeia uma propriedade de referência da entidade para o modelo.
        /// </summary>
        /// <param name="entity">Entidade com o valor da propriedade</param>
        /// <param name="modelProp">Propriedade do modelo a ser preenchida</param>
        /// <param name="entityProp">Propriedade da entidade a ser lida</param>
        /// <param name="model">Modelo a ser preenchido</param>
        private void MapEntityReference(TEntity entity, PropertyInfo modelProp, PropertyInfo entityProp, TModel model)
        {
            var referenceEntity = (IBaseEntity)entityProp.GetValue(entity);
            modelProp.SetValue(model, referenceEntity?.Id); // Mapeia o ID do relacionamento
        }

        /// <summary>
        /// Mapeia uma propriedade de referência no modelo para a entidade usando o ID.
        /// </summary>
        /// <param name="model">Modelo com o ID da referência</param>
        /// <param name="modelProp">Propriedade do modelo que contém o ID</param>
        /// <param name="entityProp">Propriedade da entidade a ser preenchida</param>
        /// <param name="entity">Entidade a ser preenchida</param>
        private void MapEntityReferenceToId(TModel model, PropertyInfo modelProp, PropertyInfo entityProp, TEntity entity)
        {
            var referenceId = (long?)modelProp.GetValue(model);
            if (referenceId.HasValue)
            {
                var referenceEntity = (IBaseEntity)Activator.CreateInstance(entityProp.PropertyType);
                referenceEntity.Id = referenceId.Value;
                entityProp.SetValue(entity, referenceEntity);
            }
            else
            {
                entityProp.SetValue(entity, null); // Remove a referência se o ID for nulo
            }
        }

        /// <summary>
        /// Deserializa o valor anterior do modelo a partir de uma string JSON.
        /// </summary>
        /// <param name="serializedOldValue">Valor serializado em JSON</param>
        /// <returns>Modelo deserializado</returns>
        private TModel DeserializeOldModel(string serializedOldValue)
        {
            return serializedOldValue != null ? JsonConvert.DeserializeObject<TModel>(serializedOldValue) : default;
        }

        /// <summary>
        /// Sincroniza as propriedades não alteradas entre o modelo antigo e o novo.
        /// </summary>
        /// <param name="model">Modelo atual</param>
        /// <param name="oldModel">Modelo antigo</param>
        /// <param name="modelFromEntity">Modelo preenchido a partir da entidade</param>
        private static void SyncUnchangedProperties(TModel model, TModel oldModel, TModel modelFromEntity)
        {
            foreach (var prop in model.GetType().GetProperties().Where(m => m.SetMethod != null && !typeof(ICollection<>).IsAssignableFrom(m.PropertyType)))
            {
                if (prop.GetValue(model)?.Equals(prop.GetValue(oldModel)) == true)
                {
                    prop.SetValue(model, prop.GetValue(modelFromEntity)); // Sincroniza valores não alterados
                }
            }
        }

        #endregion
    }
}
