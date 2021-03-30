using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Klaviyo.Models
{
    public class CategoryResponse
    {
        [JsonProperty("Id")]
        public long Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("FatherCategoryId")]
        public object FatherCategoryId { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Keywords")]
        public string Keywords { get; set; }

        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }

        [JsonProperty("LomadeeCampaignCode")]
        public string LomadeeCampaignCode { get; set; }

        [JsonProperty("AdWordsRemarketingCode")]
        public string AdWordsRemarketingCode { get; set; }

        [JsonProperty("ShowInStoreFront")]
        public bool ShowInStoreFront { get; set; }

        [JsonProperty("ShowBrandFilter")]
        public bool ShowBrandFilter { get; set; }

        [JsonProperty("ActiveStoreFrontLink")]
        public bool ActiveStoreFrontLink { get; set; }

        [JsonProperty("GlobalCategoryId")]
        public long GlobalCategoryId { get; set; }

        [JsonProperty("StockKeepingUnitSelectionMode")]
        public string StockKeepingUnitSelectionMode { get; set; }

        [JsonProperty("Score")]
        public object Score { get; set; }

        [JsonProperty("LinkId")]
        public string LinkId { get; set; }

        [JsonProperty("HasChildren")]
        public bool HasChildren { get; set; }
    }
}

