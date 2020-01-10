

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace TeisterMask.Data.Models
{
    public class Employee
    {
        public Employee()
        {
            this.EmployeesTasks = new List<EmployeeTask>();
        }

        [Key]
        public int Id { get; set; }

        [RegularExpression(@"[a-zA-Z0-9]*"),MinLength(3), MaxLength(40), Required]
        public string Username { get; set; }


        [Required, RegularExpression(@"^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$")]
        public string Email { get; set; }
        //TODO Possible mistake with REGEX

        [Required, RegularExpression(@"[0-9]{3}-[0-9]{3}-[0-9]{4}")]
        public string Phone { get; set; }
        //TODO Check Regex for some bugs

        public ICollection<EmployeeTask> EmployeesTasks  { get; set; }



    }
}
