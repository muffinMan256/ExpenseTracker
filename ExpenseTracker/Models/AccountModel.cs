using Microsoft.VisualStudio.TextTemplating;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace ExpenseTracker.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        [Column(TypeName = "nvarchar(50)")]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Password { get; set; }


        public bool RememberMe { get; set; }
        
    }

    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        [Column(TypeName = "nvarchar(50)")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Column(TypeName = "nvarchar(50)")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        //compare below
        [Compare("Password", ErrorMessage = "The password is not the same")]
        public string PasswordConfirmation { get; set; }


    }
}
