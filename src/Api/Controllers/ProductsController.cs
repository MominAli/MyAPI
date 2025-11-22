using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    // Api/Controllers/v1/ProductsController.cs

    [ApiController]
    [Route("api/v1/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IUnitOfWork uow, ILogger<ProductsController> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        [HttpGet]
        [Authorize] // authenticated users
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll([FromQuery] QueryParams qp, CancellationToken ct)
        {
            var query = await _uow.Products.ListAsync(p =>
                (string.IsNullOrEmpty(qp.Search) || p.Name.Contains(qp.Search)) &&
                (qp.MinPrice == null || p.Price >= qp.MinPrice) &&
                (qp.MaxPrice == null || p.Price <= qp.MaxPrice), ct);

            var sorted = qp.Sort switch
            {
                "price" => query.OrderBy(p => p.Price),
                "-price" => query.OrderByDescending(p => p.Price),
                "name" => query.OrderBy(p => p.Name),
                "-name" => query.OrderByDescending(p => p.Name),
                _ => query.OrderByDescending(p => p.Id)
            };

            var page = sorted.Skip((qp.Page - 1) * qp.PageSize).Take(qp.PageSize).ToList();
            return Ok(page.Select(p => new ProductDto(p)));
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken ct)
        {
            var product = await _uow.Products.GetByIdAsync(id, ct);
            if (product is null) return NotFound();
            return Ok(new ProductDto(product));
        }

        [HttpPost]
        [Authorize(Policy = "CanManageProducts")]
        public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto, CancellationToken ct)
        {
            var entity = new Product { Sku = dto.Sku, Name = dto.Name, Price = dto.Price, Stock = dto.Stock };
            await _uow.Products.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id, version = "1.0" }, new ProductDto(entity));
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "CanManageProducts")]
        public async Task<IActionResult> Update(int id, UpdateProductDto dto, CancellationToken ct)
        {
            var entity = await _uow.Products.GetByIdAsync(id, ct);
            if (entity is null) return NotFound();
            entity.Name = dto.Name ?? entity.Name;
            entity.Price = dto.Price ?? entity.Price;
            entity.Stock = dto.Stock ?? entity.Stock;

            _uow.Products.Update(entity);
            await _uow.SaveChangesAsync(ct);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var entity = await _uow.Products.GetByIdAsync(id, ct);
            if (entity is null) return NotFound();
            _uow.Products.Remove(entity);
            await _uow.SaveChangesAsync(ct);
            return NoContent();
        }
    }

}
