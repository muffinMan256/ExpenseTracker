using Microsoft.VisualStudio.TextTemplating;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace ExpenseTracker.Models
{

    public class LoginModel
    {
        [Required(ErrorMessage = "This field is mandatory!")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is mandatory!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "This field is mandatory!")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is mandatory!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "This field is mandatory!")]
        [DataType(DataType.Password)]
        //compare below
        [Compare("Password", ErrorMessage = "The password is not the same")]
        public string PasswordConfirmation { get; set; }


    }

    public class LoginViewModel
    {
        public Tuple<RegisterModel, LoginModel> UserModel { get; set; }
    }

    public class LoginRegisterCombined
    {
        public LoginModel? LoginModel { get; set; }

        public RegisterModel? RegisterModel { get; set; }
    }
}

