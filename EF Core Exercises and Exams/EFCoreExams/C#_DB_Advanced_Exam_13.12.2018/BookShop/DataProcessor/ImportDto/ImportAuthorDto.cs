using BookShop.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookShop.DataProcessor.ImportDto
{
    public class ImportAuthorDto
    {
        [JsonProperty("FirstName")]
        [MinLength(3), MaxLength(30), Required]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        [MinLength(3), MaxLength(30), Required]
        public string LastName { get; set; }

        [JsonProperty("Phone")]
        [RegularExpression(@"[0-9]{3}-[0-9]{3}-[0-9]{4}"), Required]
        public string Phone { get; set; }

        [JsonProperty("Email")]
        [EmailAddress, Required]
        public string Email { get; set; }

        [JsonProperty("Books")]
        public ICollection<ImportAuthorBookDto> Books { get; set; } = new List<ImportAuthorBookDto>();
    }
}
