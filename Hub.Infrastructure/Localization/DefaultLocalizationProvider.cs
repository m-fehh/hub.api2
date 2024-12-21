using System.Globalization;
using Hub.Infrastructure.Localization.Interfaces;

namespace Hub.Infrastructure.Localization
{
    public class DefaultLocalizationProvider : ILocalizationProvider
    {
        private static List<IResourceWrapper> _resourceWrappers = new List<IResourceWrapper>();

        public void RegisterWrapper(IResourceWrapper wrapper)
        {
            _resourceWrappers.Add(wrapper);
        }

        public string Get(string key)
        {
            return Get(key, CultureInfo.CurrentCulture);
        }

        public string Get(string key, CultureInfo culture)
        {
            var chave = string.Concat(key, "-", culture.Name);

            string textResource = null;

            foreach (var wrapper in _resourceWrappers)
            {
                textResource = wrapper.GetString(key, culture.Name);

                if (textResource != key) break;
            }

            if (textResource == null) textResource = key;

            return textResource;
        }

        public string GetByValue(string value)
        {
            string key = null;

            foreach (var wrapper in _resourceWrappers)
            {
                key = wrapper.GetByValue(value);

                if (value != key) break;
            }

            if (key == null) return value;

            return key;
        }
    }
}
