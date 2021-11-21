using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Banking.Site.Models
{
    public class LoginViewModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "The value {0} should be at least {1} symbols.", MinimumLength = 1)]
        [DataType(DataType.Text)]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The value {0} should be at least {1} symbols.", MinimumLength = 1)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "The value {0} should be at least {1} symbols.", MinimumLength = 1)]
        [DataType(DataType.Text)]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The value {0} should be at least {1} symbols.", MinimumLength = 1)]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(4, ErrorMessage = "The value {0} should be at least {1} symbols.", MinimumLength = 4)]
        [DataType(DataType.Text)]
        [Display(Name = "Pin")]
        public string Pin { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The value {0} should be at least {1} symbols.", MinimumLength = 1)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirmation are different.")]
        public string ConfirmPassword { get; set; }
    }
}
