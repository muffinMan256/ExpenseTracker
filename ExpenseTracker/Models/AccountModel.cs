using Microsoft.VisualStudio.TextTemplating;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace ExpenseTracker.Models
{

    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        public bool RememberMe { get; set; }

    }

    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
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

