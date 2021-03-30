using Klaviyo.Data;
using Klaviyo.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Vtex.Api.Context;

namespace Klaviyo.Services
{
    /// <summary>
    /// https://www.klaviyo.com/docs
    /// </summary>
    public class KlaviyoAPI : IKlaviyoAPI
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderFeedAPI _orderFeedAPI;
        private readonly IIOServiceContext _context;

        public KlaviyoAPI(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, IOrderFeedAPI orderFeedAPI, IIOServiceContext context)
        {
          this._httpContextAccessor = httpContextAccessor ??
                                      throw new ArgumentNullException(nameof(httpContextAccessor));

          this._clientFactory = clientFactory ??
                                throw new ArgumentNullException(nameof(clientFactory));

          this._orderFeedAPI = orderFeedAPI ??
                                throw new ArgumentNullException(nameof(orderFeedAPI));

          this._context = context ??
                                throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> ProcessNotification(HookNotification hookNotification)
        {
            bool success = false;
            // Use our server - side Track API for the following:
            // Placed Order - When an order successfully processes on your system
            // Ordered Product - An event for each item in a processed order
            // Fulfilled Order - When an order is sent to the customer
            // Cancelled Order - When a customer cancels their order
            // Refunded Order - When a customer’s order is refunded
            string orderId = hookNotification.OrderId;
            DateTimeOffset lastChange = hookNotification.LastChange;
            KlaviyoEvent klaviyoEvent;
            VtexOrder vtexOrder = await _orderFeedAPI.GetOrderInformation(orderId);
            switch (hookNotification.Domain)
            {
                case Constants.Domain.Fulfillment:
                    switch (hookNotification.State)
                    {
                        case Constants.Status.ReadyForHandling:
                            klaviyoEvent = await BuildEvent(vtexOrder, Constants.Events.PlacedOrder, lastChange);
                            if (klaviyoEvent != null)
                            {
                                success = await SendEvent(klaviyoEvent);
                                // Send each item as a separate event
                                foreach(Item item in vtexOrder.Items)
                                {
                                    VtexOrder order = vtexOrder;
                                    order.Items = new List<Item> { item };
                                    klaviyoEvent = await BuildEvent(vtexOrder, Constants.Events.OrderedProduct, lastChange);
                                    success = success && await SendEvent(klaviyoEvent);
                                }
                            }

                            break;
                        case Constants.Status.Invoiced:
                            klaviyoEvent = await BuildEvent(vtexOrder, Constants.Events.FulfilledOrder, lastChange);
                            if (klaviyoEvent != null)
                            {
                                success = await SendEvent(klaviyoEvent);
                            }

                            break;
                        //case Constants.Status.Canceled:
                        //    break;
                        default:
                            Console.WriteLine($"State {hookNotification.State} not implemeted.");
                            _context.Vtex.Logger.Info("ProcessNotification", null, $"State {hookNotification.State} not implemeted.");
                            break;
                    }
                    break;
                case Constants.Domain.Marketplace:
                    break;
                default:
                    Console.WriteLine($"Domain {hookNotification.Domain} not implemeted.");
                    _context.Vtex.Logger.Info("ProcessNotification", null, $"Domain {hookNotification.Domain} not implemeted.");
                    break;
            }

            return success;
        }

        public async Task<bool> ProcessNotification(AllStatesNotification allStatesNotification)
        {
            bool success = true;
            // Use our server - side Track API for the following:
            // Placed Order - When an order successfully processes on your system
            // Ordered Product - An event for each item in a processed order
            // Fulfilled Order - When an order is sent to the customer
            // Cancelled Order - When a customer cancels their order
            // Refunded Order - When a customer’s order is refunded
            string orderId = allStatesNotification.OrderId;
            DateTimeOffset lastChange = allStatesNotification.LastChangeDate;
            KlaviyoEvent klaviyoEvent;
            VtexOrder vtexOrder = await _orderFeedAPI.GetOrderInformation(orderId);
            switch (allStatesNotification.Domain)
            {
                case Constants.Domain.Fulfillment:
                    switch (allStatesNotification.CurrentState)
                    {
                        case Constants.Status.ReadyForHandling:
                            klaviyoEvent = await BuildEvent(vtexOrder, Constants.Events.PlacedOrder, lastChange);
                            if (klaviyoEvent != null)
                            {
                                success = await SendEvent(klaviyoEvent);
                                // Send each item as a separate event
                                foreach (Item item in vtexOrder.Items)
                                {
                                    VtexOrder order = vtexOrder;
                                    order.Items = new List<Item> { item };
                                    klaviyoEvent = await BuildEvent(vtexOrder, Constants.Events.OrderedProduct, lastChange);
                                    success = success && await SendEvent(klaviyoEvent);
                                }
                            }

                            break;
                        case Constants.Status.Invoiced:
                            klaviyoEvent = await BuildEvent(vtexOrder, Constants.Events.FulfilledOrder, lastChange);
                            if (klaviyoEvent != null)
                            {
                                success = await SendEvent(klaviyoEvent);
                            }

                            break;
                        case Constants.Status.Canceled:
                            klaviyoEvent = await BuildEvent(vtexOrder, Constants.Events.Canceled, lastChange);
                            if (klaviyoEvent != null)
                            {
                                success = await SendEvent(klaviyoEvent);
                            }

                            break;
                        default:
                            Console.WriteLine($"State {allStatesNotification.CurrentState} not implemeted.");
                            _context.Vtex.Logger.Info("ProcessNotification", null, $"State {allStatesNotification.CurrentState} not implemeted.");
                            break;
                    }
                    break;
                case Constants.Domain.Marketplace:
                    break;
                default:
                    Console.WriteLine($"Domain {allStatesNotification.Domain} not implemeted.");
                    _context.Vtex.Logger.Info("ProcessNotification", null, $"Domain {allStatesNotification.Domain} not implemeted.");
                    break;
            }

            return success;
        }

        public async Task<bool> SendEvent(KlaviyoEvent klaviyoEvent)
        {
            bool success = false;
            string jsonSerializedEvent = JsonConvert.SerializeObject(klaviyoEvent);
            Console.WriteLine($"Event = {jsonSerializedEvent}");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{Constants.KlaviyoAPIEndpoint}{Constants.KlaviyoTrack}?data={this.EncodeTo64(jsonSerializedEvent)}"),
                //Content = new StringContent(jsonSerializedEvent, Encoding.UTF8, Constants.APPLICATION_JSON)
            };

            request.Headers.Add(Constants.HTTP_FORWARDED_HEADER, this._httpContextAccessor.HttpContext.Request.Headers[Constants.FORWARDED_HEADER].ToString());
            string authToken = this._httpContextAccessor.HttpContext.Request.Headers[Constants.HEADER_VTEX_CREDENTIAL];
            if (authToken != null)
            {
                request.Headers.Add(Constants.AUTHORIZATION_HEADER_NAME, authToken);
                //request.Headers.Add(Constants.VtexIdCookie, authToken);
            }

            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[-] SendEvent Response {response.StatusCode} Content = '{responseContent}' [-]");
            _context.Vtex.Logger.Debug("SendEvent", null, $"{jsonSerializedEvent} [{response.StatusCode}] {responseContent}");

            // Responses from requests made to the Identify or Track APIs will return either 0 or 1.
            // A 1 response means your data was received successfully and is queued for processing.
            // A 0 response means your data did not decode to valid JSON and was not queued for processing. 
            if(response.IsSuccessStatusCode)
            {
                success = responseContent.Equals("1");
            }

            return success;
        }

        public async Task<KlaviyoEvent> BuildEvent(VtexOrder vtexOrder, string eventType, DateTimeOffset lastChange)
        {
            KlaviyoEvent klaviyoEvent = null;
            if (vtexOrder != null)
            {
                MerchantSettings merchantSettings = await _orderFeedAPI.GetMerchantSettings();
                if (eventType.Equals(Constants.Events.PlacedOrder) ||
                    eventType.Equals(Constants.Events.Canceled) ||
                    eventType.Equals(Constants.Events.FulfilledOrder))
                {
                    List<string> categoryNames = new List<string>();
                    List<string> categoryIds = vtexOrder.Items.Select(i => i.AdditionalInfo.CategoriesIds.Replace(@"\", string.Empty)).ToList();
                    foreach (string categoryId in categoryIds)
                    {
                        CategoryResponse categoryResponse = await _orderFeedAPI.GetCategoryById(categoryId);
                        if (categoryResponse != null)
                        {
                            categoryNames.Add(categoryResponse.Name);
                        }
                    }

                    klaviyoEvent = new KlaviyoEvent
                    {
                        CustomerProperties = new CustomerProperties
                        {
                            //Address1 = vtexOrder.ShippingData.Address.Street,
                            //Address2 = vtexOrder.ShippingData.Address.Number,
                            //City = vtexOrder.ShippingData.Address.City,
                            //Country = vtexOrder.ShippingData.Address.Country,
                            Email = vtexOrder.ClientProfileData.Email,
                            FirstName = vtexOrder.ClientProfileData.FirstName, //vtexOrder.ShippingData.Address.FirstName,
                            LastName = vtexOrder.ClientProfileData.LastName, //vtexOrder.ShippingData.Address.LastName,
                            PhoneNumber = vtexOrder.ClientProfileData.Phone, //vtexOrder.ShippingData.Address.Phone,
                            //Region = vtexOrder.ShippingData.Address.State,
                            //Zip = vtexOrder.ShippingData.Address.PostalCode
                        },
                        Event = eventType,
                        Properties = new Properties
                        {
                            Brands = vtexOrder.Items.Select(i => i.AdditionalInfo.BrandName).ToList(),
                            Categories = categoryNames,
                            DiscountCode = string.Join(',', vtexOrder.RatesAndBenefitsData.RateAndBenefitsIdentifiers.Select(r => r.Name).ToList()),
                            DiscountValue = vtexOrder.Totals.Where(t => t.Name == "Discounts").Select(d => d.Value).FirstOrDefault(),
                            EventId = vtexOrder.OrderId,
                            ItemNames = vtexOrder.Items.Select(i => i.Name).ToList(),
                            Items = new List<KlaviyoItem>(),
                            Value = ToDollar(vtexOrder.Totals.Sum(t => t.Value)),
                            BillingAddress = new Address
                            {
                                //Address1 = vtexOrder.ShippingData.Address.Street,
                                //Address2 = vtexOrder.ShippingData.Address.Number,
                                //City = vtexOrder.ShippingData.Address.City,
                                //Country = vtexOrder.ShippingData.Address.Country,
                                FirstName = vtexOrder.ClientProfileData.FirstName, //vtexOrder.ShippingData.Address.FirstName,
                                LastName = vtexOrder.ClientProfileData.LastName, //vtexOrder.ShippingData.Address.LastName,
                                //Region = vtexOrder.ShippingData.Address.State,
                                //Zip = vtexOrder.ShippingData.Address.PostalCode,
                                Company = null,
                                //CountryCode = vtexOrder.ShippingData.Address.Country,
                                Phone = vtexOrder.ClientProfileData.Phone,
                                //RegionCode = vtexOrder.ShippingData.Address.State
                            },
                            ShippingAddress = new Address
                            {
                                Address1 = vtexOrder.ShippingData.Address.Street,
                                Address2 = vtexOrder.ShippingData.Address.Number,
                                City = vtexOrder.ShippingData.Address.City,
                                Country = vtexOrder.ShippingData.Address.Country,
                                //FirstName = vtexOrder.ClientProfileData.FirstName, //vtexOrder.ShippingData.Address.FirstName,
                                //LastName = vtexOrder.ClientProfileData.LastName, //vtexOrder.ShippingData.Address.LastName,
                                Region = vtexOrder.ShippingData.Address.State,
                                Zip = vtexOrder.ShippingData.Address.PostalCode,
                                Company = null,
                                CountryCode = vtexOrder.ShippingData.Address.Country,
                                Phone = vtexOrder.ClientProfileData.Phone,
                                RegionCode = vtexOrder.ShippingData.Address.State
                            }
                        },
                        //Time = lastChange.UtcTicks,
                        Token = merchantSettings.AppId
                    };

                    foreach (Item item in vtexOrder.Items)
                    {
                        KlaviyoItem klaviyoItem = new KlaviyoItem
                        {
                            Brand = item.AdditionalInfo.BrandName,
                            Categories = new List<string>(),
                            ImageUrl = item.ImageUrl.AbsoluteUri,
                            ItemPrice = ToDollar(item.Price),
                            ProductId = item.ProductId,
                            ProductName = item.Name,
                            ProductUrl = $"{merchantSettings.ProductRootUrl}{item.DetailUrl}",
                            Quantity = item.Quantity,
                            RowTotal = ToDollar(item.Price * item.Quantity),
                            Sku = item.SellerSku
                        };

                        CategoryResponse categoryResponse = await _orderFeedAPI.GetCategoryById(item.AdditionalInfo.CategoriesIds.ToString().Replace(@"/", string.Empty));
                        klaviyoItem.Categories.Add(categoryResponse.Name);
                        klaviyoEvent.Properties.Items.Add(klaviyoItem);
                    }

                    if(eventType.Equals(Constants.Events.Canceled))
                    {
                        klaviyoEvent.Properties.Reason = vtexOrder.CancelReason;
                    }
                }
                else if(eventType.Equals(Constants.Events.OrderedProduct))
                {
                    klaviyoEvent = new KlaviyoEvent
                    {
                        Token = merchantSettings.AppId,
                        Event = eventType,
                        CustomerProperties = new CustomerProperties
                        {
                            Email = vtexOrder.ClientProfileData.Email,
                            FirstName = vtexOrder.ClientProfileData.FirstName,
                            LastName = vtexOrder.ClientProfileData.LastName
                        },
                        Properties = new Properties
                        {
                            EventId = vtexOrder.OrderId,
                            Value = ToDollar(vtexOrder.Items[0].Price),     //ToDollar(vtexOrder.Items[0].Price * vtexOrder.Items[0].Quantity),
                            OrderId = vtexOrder.OrderId,
                            ProductId = vtexOrder.Items[0].ProductId,
                            Sku = vtexOrder.Items[0].SellerSku,
                            ProductName = vtexOrder.Items[0].Name,
                            Quantity = vtexOrder.Items[0].Quantity,
                            ProductUrl = $"{merchantSettings.ProductRootUrl}{vtexOrder.Items[0].DetailUrl}",
                            ImageUrl = vtexOrder.Items[0].ImageUrl.AbsoluteUri,
                            Categories = new List<string>(),
                            ProductBrand = vtexOrder.Items[0].AdditionalInfo.BrandName
                        },
                        //Time = 
                    };

                    CategoryResponse categoryResponse = await _orderFeedAPI.GetCategoryById(vtexOrder.Items[0].AdditionalInfo.CategoriesIds.ToString().Replace(@"/", string.Empty));
                    klaviyoEvent.Properties.Categories.Add(categoryResponse.Name);
                }

                _context.Vtex.Logger.Debug("BuildEvent", eventType, JsonConvert.SerializeObject(klaviyoEvent));
            }
            else
            {
                Console.WriteLine($"Could not load order {vtexOrder.OrderId}");
                _context.Vtex.Logger.Warn("BuildEvent", null, $"Could not load order {vtexOrder.OrderId}");
            }

            return klaviyoEvent;
        }

        public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue = Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;
        }

        public decimal ToDollar(int asPennies)
        {
            return (decimal)asPennies / 100;
        }
    }
}
