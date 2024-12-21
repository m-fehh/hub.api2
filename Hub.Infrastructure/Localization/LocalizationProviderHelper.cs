using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace Hub.Infrastructure.Localization
{
    public static class LocalizationProviderHelper
    {
        private static string GetDisplayName<TSource, TProperty>(TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            var displayName = propInfo.Name;

            var displayAttribute = propInfo.GetCustomAttribute(typeof(DisplayAttribute));

            if (displayAttribute != null)
            {
                displayName = (displayAttribute as DisplayAttribute).Name;
            }

            return displayName;
        }

        public static string DefaultRequiredMessage<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var displayName = GetDisplayName(source, propertyLambda);

            return string.Format(Engine.Get("DefaultRequiredMessage"), Engine.Get(displayName));
        }

        public static string DefaultAlreadyRegisteredMessage<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var displayName = GetDisplayName(source, propertyLambda);

            return string.Format(Engine.Get("DefaultAlreadyRegistered"), Engine.Get(displayName));
        }

        public static string DisplayName<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            return GetDisplayName(source, propertyLambda);
        }
    }
}
