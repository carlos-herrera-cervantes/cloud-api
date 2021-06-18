using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Api.Web.Extensions
{
    public static class HeaderDictionaryExtensions
    {
        public static string ExtractJsonWebToken(this IHeaderDictionary headers)
        {
            var authorization = headers["Authorization"].ToString();
            var token = authorization?.Split(" ").LastOrDefault();

            return token;
        }
    }
}
