
using System.Collections.Generic;

namespace ProductShop.Models.ModelDto
{
    public class UserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<SoldProductDto> SoldProducts { get; set; }
    }
}
