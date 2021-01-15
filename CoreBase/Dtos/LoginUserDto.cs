using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBase.Dtos
{
    public class LoginUserDto
    {
        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; }

        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?!.*(.)\1\1)[a-zA-Z0-9@]{6,12}$",
            ErrorMessage = "Password need an uppercare, a special and a numeric characters with only two repetitive characters")]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
