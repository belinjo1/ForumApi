using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ForumApplication.DTOs
{
    public class LoginDTO
    {
        //[Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        //[Required]
        [StringLength(15, ErrorMessage = "Password must have a minimum of 8 letters", MinimumLength = 8)]
        public string Password { get; set; }
    }
    public class UserDTO : LoginDTO
    {
        [Required]
        [StringLength(15, ErrorMessage = "Name must have a minimum of 2 letters", MinimumLength = 2)]
        public string Name { get; set; }
        [Required]
        [StringLength(15, ErrorMessage = "Last Name must have a minimum of 2 letters", MinimumLength = 2)]
        public string LastName { get; set; }
    }

}
