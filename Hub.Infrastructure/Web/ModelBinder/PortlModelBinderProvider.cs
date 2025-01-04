using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hub.Infrastructure.Web.ModelBinder
{
    public class PortlModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType == typeof(decimal) || context.Metadata.ModelType == typeof(decimal?))
            {
                return new BinderTypeModelBinder(typeof(DecimalModelBinder));
            }

            if (context.Metadata.ModelType == typeof(double) || context.Metadata.ModelType == typeof(double?))
            {
                return new BinderTypeModelBinder(typeof(DoubleModelBinder));
            }

            return null;
        }
    }
}
