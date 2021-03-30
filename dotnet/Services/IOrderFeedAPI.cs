using System.Threading.Tasks;
using Klaviyo.Models;

namespace Klaviyo.Services
{
    public interface IOrderFeedAPI
    {
        Task<HookNotification> CreateOrUpdateHook();
        Task<VtexOrder> GetOrderInformation(string orderId);
        Task<MerchantSettings> GetMerchantSettings();
        Task<CategoryResponse> GetCategoryById(string categoryId);
    }
}