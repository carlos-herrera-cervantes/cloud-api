using Microsoft.AspNetCore.Mvc;

namespace Api.Domain.Models
{
    public class ListResourceRequest
    {
        #region snippet_PrivateProperties

        private int _pageSize;

        #endregion

        #region snippet_Properties

        [FromQuery(Name = "sort")]
        public string Sort { get; set; } = null;

        [FromQuery(Name = "pageSize")]
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value < 1)
                {
                    _pageSize = 10;
                }
                else if (value > 100)
                {
                    _pageSize = 100;
                }
                else
                {
                    _pageSize = value;
                }
            }
        }

        [FromQuery(Name = "page")]
        public int Page { get; set; } = 0;

        [FromQuery(Name = "relation")]
        public string Entities { get; set; } = null;

        [FromQuery(Name = "filter")]
        public string Filters { get; set; } = null;

        #endregion

        #region snippet_Deconstruct

        public void Deconstruct
        (
            out string sort,
            out int pageSize,
            out int page,
            out string entities,
            out string filters
        )
        {
            sort = Sort;
            pageSize = PageSize;
            page = Page;
            entities = Entities;
            filters = Filters;
        }

        public void Deconstruct(out string sort, out string filters) => (sort, filters) = (Sort, Filters);

        #endregion
    }

    public class SingleResourceRequest
    {
        [FromQuery(Name = "relation")]
        public string Entities { get; set; } = null;
    }
}