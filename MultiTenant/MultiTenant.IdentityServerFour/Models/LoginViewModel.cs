using System.ComponentModel.DataAnnotations;

namespace MultiTenant.IdentityServerFour.Models
{
    public class LoginViewModel
    {

        [Required]
        public string UserEmail { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}