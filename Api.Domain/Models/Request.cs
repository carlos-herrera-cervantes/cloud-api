using Microsoft.AspNetCore.Mvc;

namespace Api.Domain.Models
{
    public class Request
    {
        #region snippet_Properties

        [FromQuery(Name = "sort")]
        public string Sort { get; set; }

        [FromQuery(Name = "pageSize")]
        public int PageSize { get; set; } = 10;

        [FromQuery(Name = "page")]
        public int Page { get; set; } = 1;

        [FromQuery(Name = "paginate")]
        public bool Paginate { get; set; } = false;

        [FromQuery(Name = "relation")]
        public string[] Entities { get; set; } = new [] { "Empty" };

        [FromQuery(Name = "filter")]
        public string[] Filters { get; set; }

        #endregion

        #region snippet_Deconstruct

        public void Deconstruct(out string sort, out int pageSize, out int page, out bool paginate, out string[] entities, out string[] filters)
        {
            sort = Sort;
            pageSize = PageSize;
            page = Page;
            paginate = Paginate;
            entities = Entities;
            filters = Filters;
        }

        public void Deconstruct(out bool paginate, out string[] filters) => (paginate, filters) = (Paginate, Filters);

        #endregion
    }
}