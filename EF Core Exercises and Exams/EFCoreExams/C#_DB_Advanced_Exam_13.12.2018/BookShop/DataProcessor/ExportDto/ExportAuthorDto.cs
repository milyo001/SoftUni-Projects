using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookShop.DataProcessor.ExportDto
{
    public class ExportAuthorDto
    {
        [JsonProperty("AuthorName")]
        public string AuthorName { get; set; }

        [JsonProperty("Books")]
        public ICollection<ExportBookDto> Books { get; set; }
    }
}
