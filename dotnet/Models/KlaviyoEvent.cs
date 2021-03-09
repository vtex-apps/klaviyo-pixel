using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Klaviyo.Models
{

    public class KlaviyoEvent
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("customer_properties")]
        public CustomerProperties CustomerProperties { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("billing_address")]
        public Address BillingAddress { get; set; }

        [JsonProperty("shipping_address")]
        public Address ShippingAddress { get; set; }
    }

    public class CustomerProperties
    {
        [JsonProperty("$email")]
        public string Email { get; set; }

        [JsonProperty("$first_name")]
        public string FirstName { get; set; }

        [JsonProperty("$last_name")]
        public string LastName { get; set; }

        [JsonProperty("$phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("$address1")]
        public string Address1 { get; set; }

        [JsonProperty("$address2")]
        public string Address2 { get; set; }

        [JsonProperty("$city")]
        public string City { get; set; }

        [JsonProperty("$zip")]
        public string Zip { get; set; }

        [JsonProperty("$region")]
        public string Region { get; set; }

        [JsonProperty("$country")]
        public string Country { get; set; }
    }

    public class Properties
    {
        [JsonProperty("$event_id")]
        public string EventId { get; set; }

        [JsonProperty("$value")]
        public decimal Value { get; set; }

        [JsonProperty("Categories")]
        public List<string> Categories { get; set; }

        [JsonProperty("ItemNames")]
        public List<string> ItemNames { get; set; }

        [JsonProperty("Brands")]
        public List<string> Brands { get; set; }

        [JsonProperty("Discount Code")]
        public string DiscountCode { get; set; }

        [JsonProperty("Discount Value")]
        public decimal DiscountValue { get; set; }

        [JsonProperty("Items")]
        public List<KlaviyoItem> Items { get; set; }
    }

    public class KlaviyoItem
    {
        [JsonProperty("ProductID")]
        public string ProductId { get; set; }

        [JsonProperty("SKU")]
        public string Sku { get; set; }

        [JsonProperty("ProductName")]
        public string ProductName { get; set; }

        [JsonProperty("Quantity")]
        public int Quantity { get; set; }

        [JsonProperty("ItemPrice")]
        public decimal ItemPrice { get; set; }

        [JsonProperty("RowTotal")]
        public decimal RowTotal { get; set; }

        [JsonProperty("ProductURL")]
        public string ProductUrl { get; set; }

        [JsonProperty("ImageURL")]
        public string ImageUrl { get; set; }

        [JsonProperty("Categories")]
        public Dictionary<string, string> Categories { get; set; }

        [JsonProperty("Brand")]
        public string Brand { get; set; }

    }

    public class Address
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("company")]
        public string Company { get; set; }

        [JsonProperty("address1")]
        public string Address1 { get; set; }

        [JsonProperty("address2")]
        public string Address2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("region_code")]
        public string RegionCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }
    }
}
