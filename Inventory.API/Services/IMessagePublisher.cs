using System.Threading.Tasks;
using Inventory.API.Models;

namespace Inventory.API.Services
{
    public interface IMessagePublisher
    {
        Task PublishProductUpdated(Product product);
        Task PublishProductDeleted(int productId);
    }
}
