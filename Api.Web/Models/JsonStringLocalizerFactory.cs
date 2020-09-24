using System;
using Microsoft.Extensions.Localization;

namespace Api.Web.Models
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        #region snippet_CreateByResource

        public IStringLocalizer Create(Type resourceSource) => new JsonStringLocalizer();

        #endregion

        #region snippet_CreateByBaseNameAndLocation

        public IStringLocalizer Create(string baseName, string location) => new JsonStringLocalizer();

        #endregion
    }
}