using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory.Shared.Messages;
using Inventory.Shared.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IBus bus) : ControllerBase
    {
        private readonly IBus _bus = bus;
        private static readonly List<Product> _products = [];
        private static int _nextId = 1;

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            return Ok(_products);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            var product = _products.Find(p => p.Id == id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            product.Id = _nextId++;
            _products.Add(product);

            var notification = new ProductCreatedMessage()
            {
                EventType = "created",
                ProductName = product.Name,
                Quantity = product.Quantity,
                Details = $"Product {product.Name} was created",
            };

            await _bus.Publish(
                notification,
                context =>
                {
                    context.SetRoutingKey("inventory.created");
                }
            );

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            var existingProduct = _products.Find(p => p.Id == id);
            if (existingProduct == null)
                return NotFound();

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Quantity = product.Quantity;
            existingProduct.Category = product.Category;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            var notification = new ProductUpdatedMessage()
            {
                EventType = "updated",
                ProductName = product.Name,
                Quantity = product.Quantity,
                Details = $"Product {product.Name} was updated",
            };

            await _bus.Publish(
                notification,
                context =>
                {
                    context.SetRoutingKey("inventory.updated");
                }
            );

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = _products.Find(p => p.Id == id);
            if (product == null)
                return NotFound();

            _products.Remove(product);
            var notification = new ProductDeletedMessage()
            {
                EventType = "deleted",
                ProductName = product.Name,
                Quantity = product.Quantity,
                Details = $"Product {product.Name} was deleted",
            };

            await _bus.Publish(
                notification,
                context =>
                {
                    context.SetRoutingKey("inventory.deleted");
                }
            );

            return NoContent();
        }
    }
}
