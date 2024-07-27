using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Authentication;


namespace ExpenseTracker.Models
{
    public class LoginModel
    {
        [Display(Name = "E-Mail")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "This field is mandatory!")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "This field is mandatory!")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        //[NotMapped]
        //public string ReturnUrl { get; set; }
        //[NotMapped]
        //public IList<AuthenticationScheme>? ExternalLogins { get; set; }

    }
    public class RegisterModel
    {
        [Required(ErrorMessage = "This field is mandatory!")]
        [Display(Name = "E-Mail")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is mandatory!")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "This field is mandatory!")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "This field is mandatory!")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        //compare below
        [Compare("Password", ErrorMessage = "The password is not the same")]
        public string PasswordConfirmation { get; set; }
    }
}

