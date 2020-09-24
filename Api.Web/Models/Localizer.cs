using System.Collections.Generic;

namespace Api.Web.Models
{
    public class Localizer
    {
      public string Key { get; set; }
      public Dictionary<string, string> LocalizedValue { get; set; }
    }
}