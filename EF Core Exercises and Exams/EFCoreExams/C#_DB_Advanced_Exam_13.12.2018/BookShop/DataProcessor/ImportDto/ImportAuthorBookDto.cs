using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookShop.DataProcessor.ImportDto
{

    public class ImportAuthorBookDto
    {
        
        [JsonProperty("Id")]
        public string Id { get; set; }
    }
}
