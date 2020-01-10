
namespace BookShop.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Author
    {

        [Key]
        public int Id { get; set; }

        [MinLength(3), MaxLength(30), Required]
        public string FirstName { get; set; }

        [MinLength(3), MaxLength(30), Required]
        public string LastName { get; set; }

        [EmailAddress, Required]
        public string Email { get; set; }

        [RegularExpression(@"[0-9]{3}-[0-9]{3}-[0-9]{4}"), Required]
        public string Phone { get; set; }

        public ICollection<AuthorBook> AuthorsBooks { get; set; } = new List<AuthorBook>();
    }
}
