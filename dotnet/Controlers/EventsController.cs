namespace service.Controllers
{
    using Klaviyo.Models;
    using Klaviyo.Services;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System;
    using Vtex.Api.Context;

    public class EventsController : Controller
    {
        private readonly IIOServiceContext _context;
        private readonly IKlaviyoAPI _klaviyoAPI;

        public EventsController(IIOServiceContext context, IKlaviyoAPI klaviyoAPI)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            this._klaviyoAPI = klaviyoAPI ?? throw new ArgumentNullException(nameof(klaviyoAPI));
        }

        public string OnAppsLinked(string account, string workspace)
        {
            return $"OnAppsLinked event detected for {account}/{workspace}";
        }

        public void AllStates(string account, string workspace)
        {
            string bodyAsText = new System.IO.StreamReader(HttpContext.Request.Body).ReadToEndAsync().Result;
            AllStatesNotification allStatesNotification = JsonConvert.DeserializeObject<AllStatesNotification>(bodyAsText);
            bool success = _klaviyoAPI.ProcessNotification(allStatesNotification).Result;
            if (!success)
            {
                _context.Vtex.Logger.Info("Order Broadcast", null, $"Failed to Process Notification {bodyAsText}");
                throw new Exception("Failed to Process Notification");
            }
        }
    }
}
