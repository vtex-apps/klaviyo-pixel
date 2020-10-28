using System;
using System.Threading.Tasks;
using Klaviyo.Models;

namespace Klaviyo.Services
{
    public interface IKlaviyoAPI
    {
        Task<KlaviyoEvent> BuildEvent(VtexOrder vtexOrder, string eventType, DateTimeOffset lastChange);
        Task<bool> ProcessNotification(HookNotification hookNotification);
        Task<bool> ProcessNotification(AllStatesNotification hookNotification);
        Task<bool> SendEvent(KlaviyoEvent klaviyoEvent);
    }
}