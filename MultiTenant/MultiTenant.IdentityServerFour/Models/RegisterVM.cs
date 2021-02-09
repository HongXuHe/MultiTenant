using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenant.IdentityServerFour.Models
{
    public class RegisterVM
    {
        [Required]
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
