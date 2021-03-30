using Klaviyo.Data;
using Klaviyo.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Vtex.Api.Context;

namespace Klaviyo.Services
{
    public class OrderFeedAPI : IOrderFeedAPI
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IVtexEnvironmentVariableProvider _environmentVariableProvider;
        private readonly IIOServiceContext _context;

        public OrderFeedAPI(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, IVtexEnvironmentVariableProvider environmentVariableProvider, IIOServiceContext context)
        {
            this._httpContextAccessor = httpContextAccessor ??
                                        throw new ArgumentNullException(nameof(httpContextAccessor));

            this._clientFactory = clientFactory ??
                                  throw new ArgumentNullException(nameof(clientFactory));

            this._environmentVariableProvider = environmentVariableProvider ??
                                                throw new ArgumentNullException(nameof(environmentVariableProvider));

            this._context = context ??
                                throw new ArgumentNullException(nameof(context));
        }

        public async Task<HookNotification> CreateOrUpdateHook()
        {
            // POST https://{accountName}.{environment}.com.br/api/orders/hook/config

            HookNotification createOrUpdateHookResponse = new HookNotification();
            OrderHook orderHook = new OrderHook
            {
                Filter = new Filter
                {
                    Status = new List<string>
                    {
                        Constants.Status.ReadyForHandling,
                        Constants.Status.Invoiced
                    }
                },
                Hook = new Hook
                {
                    Headers = new Headers
                    {
                        Key = Constants.EndPointKey
                    },
                    Url = new Uri($"https://{this._httpContextAccessor.HttpContext.Request.Headers[Constants.VTEX_ACCOUNT_HEADER_NAME]}.myvtex.com/{Constants.AppName}")
                }
            };

            var jsonSerializedOrderHook = JsonConvert.SerializeObject(orderHook);
            //Console.WriteLine($"Hook = {jsonSerializedOrderHook}");
            //Console.WriteLine($"Url = http://{this._httpContextAccessor.HttpContext.Request.Headers[Constants.VTEX_ACCOUNT_HEADER_NAME]}.{Constants.ENVIRONMENT}.com.br/api/orders/hook/config");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"http://{this._httpContextAccessor.HttpContext.Request.Headers[Constants.VTEX_ACCOUNT_HEADER_NAME]}.{Constants.ENVIRONMENT}.com.br/api/orders/hook/config"),
                Content = new StringContent(jsonSerializedOrderHook, Encoding.UTF8, Constants.APPLICATION_JSON)
            };

            string authToken = this._httpContextAccessor.HttpContext.Request.Headers[Constants.HEADER_VTEX_CREDENTIAL];
            if (authToken != null)
            {
                request.Headers.Add(Constants.AUTHORIZATION_HEADER_NAME, authToken);
                request.Headers.Add(Constants.PROXY_AUTHORIZATION_HEADER_NAME, authToken);
            }

            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[-] Response {response.StatusCode} Content = '{responseContent}' [-]");
            createOrUpdateHookResponse = JsonConvert.DeserializeObject<HookNotification>(responseContent);

            return createOrUpdateHookResponse;
        }

        public async Task<VtexOrder> GetOrderInformation(string orderId)
        {
            //Console.WriteLine("------- Headers -------");
            //foreach (var header in this._httpContextAccessor.HttpContext.Request.Headers)
            //{
            //    Console.WriteLine($"{header.Key}: {header.Value}");
            //}
            //Console.WriteLine($"http://{this._httpContextAccessor.HttpContext.Request.Headers[Constants.VTEX_ACCOUNT_HEADER_NAME]}.{Constants.ENVIRONMENT}.com.br/api/checkout/pvt/orders/{orderId}");

            VtexOrder vtexOrder = null;

            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"http://{this._httpContextAccessor.HttpContext.Request.Headers[Constants.VTEX_ACCOUNT_HEADER_NAME]}.{Constants.ENVIRONMENT}.com.br/api/checkout/pvt/orders/{orderId}")
                };

                //request.Headers.Add(Constants.ACCEPT, Constants.APPLICATION_JSON);
                //request.Headers.Add(Constants.CONTENT_TYPE, Constants.APPLICATION_JSON);
                string authToken = this._httpContextAccessor.HttpContext.Request.Headers[Constants.HEADER_VTEX_CREDENTIAL];
                //Console.WriteLine($"Token = '{authToken}'");
                if (authToken != null)
                {
                    request.Headers.Add(Constants.AUTHORIZATION_HEADER_NAME, authToken);
                    request.Headers.Add(Constants.VTEX_ID_HEADER_NAME, authToken);
                    request.Headers.Add(Constants.PROXY_AUTHORIZATION_HEADER_NAME, authToken);
                }

                //StringBuilder sb = new StringBuilder();

                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    vtexOrder = JsonConvert.DeserializeObject<VtexOrder>(responseContent);
                    Console.WriteLine($"GetOrderInformation: [{response.StatusCode}] ");
                }
                else
                {
                    Console.WriteLine($"GetOrderInformation: [{response.StatusCode}] '{responseContent}'");
                    _context.Vtex.Logger.Info("GetOrderInformation", null, $"Order# {orderId} [{response.StatusCode}] '{responseContent}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetOrderInformation Error: {ex.Message}");
                _context.Vtex.Logger.Error("GetOrderInformation", null, $"Order# {orderId} Error", ex);
            }

            return vtexOrder;
        }

        public async Task<MerchantSettings> GetMerchantSettings()
        {
            // Load merchant settings
            // 'http://apps.{{region}}.vtex.io/{{account}}/{{workspace}}/apps/{{appName}}/settings'
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"http://apps.{this._environmentVariableProvider.Region}.vtex.io/{this._httpContextAccessor.HttpContext.Request.Headers[Constants.VTEX_ACCOUNT_HEADER_NAME]}/{this._httpContextAccessor.HttpContext.Request.Headers[Constants.HEADER_VTEX_WORKSPACE]}/apps/{Constants.APP_SETTINGS}/settings"),
            };

            string authToken = this._httpContextAccessor.HttpContext.Request.Headers[Constants.HEADER_VTEX_CREDENTIAL];
            if (authToken != null)
            {
                request.Headers.Add(Constants.AUTHORIZATION_HEADER_NAME, authToken);
                request.Headers.Add(Constants.VTEX_ID_HEADER_NAME, authToken);
            }

            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<MerchantSettings>(responseContent);
        }

        public async Task<CategoryResponse> GetCategoryById(string categoryId)
        {
            // GET https://{accountName}.{environment}.com.br/api/catalog/pvt/category/categoryId

            CategoryResponse categoryResponse = null;

            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"http://{this._httpContextAccessor.HttpContext.Request.Headers[Constants.VTEX_ACCOUNT_HEADER_NAME]}.{Constants.ENVIRONMENT}.com.br/api/catalog/pvt/category/{categoryId}")
                };

                string authToken = this._httpContextAccessor.HttpContext.Request.Headers[Constants.HEADER_VTEX_CREDENTIAL];
                if (authToken != null)
                {
                    request.Headers.Add(Constants.AUTHORIZATION_HEADER_NAME, authToken);
                    request.Headers.Add(Constants.VTEX_ID_HEADER_NAME, authToken);
                    request.Headers.Add(Constants.PROXY_AUTHORIZATION_HEADER_NAME, authToken);
                }

                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    categoryResponse = JsonConvert.DeserializeObject<CategoryResponse>(responseContent);
                    Console.WriteLine($"GetCategoryById: [{response.StatusCode}] ");
                }
                else
                {
                    Console.WriteLine($"GetCategoryById: [{response.StatusCode}] '{responseContent}'");
                    _context.Vtex.Logger.Info("GetCategoryById", null, $"Category Id '{categoryId}' [{response.StatusCode}] '{responseContent}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetCategoryById Error: {ex.Message}");
                _context.Vtex.Logger.Error("GetCategoryById", null, $"Category Id '{categoryId}' Error", ex);
            }

            return categoryResponse;
        }
    }
}
