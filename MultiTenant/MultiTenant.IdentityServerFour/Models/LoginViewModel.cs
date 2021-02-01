using System.ComponentModel.DataAnnotations;

namespace MultiTenant.IdentityServerFour.Models
{
    public class LoginViewModel
    {

        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string ReturnUrl { get; set; }
    }
}