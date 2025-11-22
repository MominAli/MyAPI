using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public OrdersController(IUnitOfWork uow) => _uow = uow;

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Order>> GetById(int id, CancellationToken ct)
        {
            var order = await _uow.Orders.GetByIdAsync(id, ct);
            if (order is null) return NotFound();
            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Order order, CancellationToken ct)
        {
            await _uow.Orders.AddAsync(order, ct);
            await _uow.SaveChangesAsync(ct);
            return CreatedAtAction(nameof(GetById), new { id = order.Id, version = "1.0" }, order);
        }
    }
}
