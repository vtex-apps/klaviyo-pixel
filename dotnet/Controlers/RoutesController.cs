namespace service.Controllers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Klaviyo.Data;
    using Klaviyo.Models;
    using Klaviyo.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Vtex.Api.Context;

    public class RoutesController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderFeedAPI _orderFeedAPI;
        private readonly IKlaviyoAPI _klaviyoAPI;
        private readonly IIOServiceContext _context;

        public RoutesController(IHttpContextAccessor httpContextAccessor, IOrderFeedAPI orderFeedAPI, IKlaviyoAPI klaviyoAPI, IIOServiceContext context)
        {
            this._httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            this._orderFeedAPI = orderFeedAPI ?? throw new ArgumentNullException(nameof(orderFeedAPI));
            this._klaviyoAPI = klaviyoAPI ?? throw new ArgumentNullException(nameof(klaviyoAPI));
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IActionResult> CreateHook()
        {
            HookNotification createOrUpdateHookResponse = await this._orderFeedAPI.CreateOrUpdateHook();
            Response.Headers.Add("Cache-Control", "private");

            return Json(createOrUpdateHookResponse);
        }

        public async Task<IActionResult> ProcessNotification()
        {
            return Ok();
            bool success = false;
            ActionResult status = BadRequest();
            if ("post".Equals(HttpContext.Request.Method, StringComparison.OrdinalIgnoreCase))
            {
                string bodyAsText = await new System.IO.StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                Console.WriteLine($"[Hook Notification] : '{bodyAsText}'");
                dynamic notification = JsonConvert.DeserializeObject<dynamic>(bodyAsText);
                if (notification != null && notification.hookConfig != null && notification.hookConfig == Constants.HookPing)
                {
                    status = Ok();
                    success = true;
                }
                else
                {
                    HookNotification hookNotification = JsonConvert.DeserializeObject<HookNotification>(bodyAsText);
                    success = await _klaviyoAPI.ProcessNotification(hookNotification);
                    status = success ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
                }

                _context.Vtex.Logger.Info("ProcessNotification", null, $"Success? [{success}] for {bodyAsText}");
            }
            else
            {
                Console.WriteLine($"[Hook Notification] : '{HttpContext.Request.Method}'");
            }

            Console.WriteLine($"[Process Notification] : '{success}'");
            return Json(status);
        }

        public async Task<IActionResult> BuildEvent(string orderId, string eventType)
        {
            Console.WriteLine($"[Build Event]");
            DateTimeOffset dateTimeOffset = new DateTimeOffset(DateTime.Now);
            VtexOrder vtexOrder = await _orderFeedAPI.GetOrderInformation(orderId);
            KlaviyoEvent klaviyoEvent = await _klaviyoAPI.BuildEvent(vtexOrder, eventType, dateTimeOffset);
            return Json(klaviyoEvent);
        }
    }
}
