using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    // Application/DTOs/QueryParams.cs
    public class QueryParams
    {
        public string? Search { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Sort { get; set; } = "-id";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
