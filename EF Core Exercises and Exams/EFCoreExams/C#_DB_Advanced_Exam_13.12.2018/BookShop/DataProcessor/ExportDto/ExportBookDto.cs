using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookShop.DataProcessor.ExportDto
{
    public class ExportBookDto
    {
        [JsonProperty("BookName")]
        public string BookName { get; set; }

        [JsonProperty("BookPrice")]
        public string BookPrice { get; set; }
    }
}
