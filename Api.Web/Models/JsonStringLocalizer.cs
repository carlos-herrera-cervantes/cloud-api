using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace Api.Web.Models
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        List<Localizer> localizers = new List<Localizer>();

        public JsonStringLocalizer()
        {
            var serializer = new JsonSerializer();
            localizers = JsonConvert.DeserializeObject<List<Localizer>>(File.ReadAllText(@"localization.json"));
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetString(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        #region snippet_GetAll

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return localizers
                .Where(l => l.LocalizedValue.Keys.
                    Any(lv => lv == CultureInfo.CurrentCulture.Name))
                    .Select(l => new LocalizedString(l.Key, l.LocalizedValue[CultureInfo.CurrentCulture.Name], true));
        }

        #endregion

        #region snippet_CreateJsonStringLocalizer

        public IStringLocalizer WithCulture(CultureInfo culture) => new JsonStringLocalizer();

        #endregion

        #region snippet_GetString

        private string GetString(string name)
        {
            var query = localizers.Where(l => l.LocalizedValue.Keys.Any(lv => lv == CultureInfo.CurrentCulture.Name));
            var value = query.FirstOrDefault(l => l.Key == name);
            return value.LocalizedValue[CultureInfo.CurrentCulture.Name];
        }

        #endregion
    }
}