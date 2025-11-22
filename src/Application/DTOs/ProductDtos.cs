using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    // Application/DTOs/ProductDtos.cs
    public record ProductDto(int Id, string Sku, string Name, decimal Price, int Stock)
    {
        public ProductDto(Product p) : this(p.Id, p.Sku, p.Name, p.Price, p.Stock) { }
    }
    public record CreateProductDto(string Sku, string Name, decimal Price, int Stock);
    public record UpdateProductDto(string? Name, decimal? Price, int? Stock);
}
