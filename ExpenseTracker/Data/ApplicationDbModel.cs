using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Data
{
    public class AppUser : IdentityUser
    {
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "Select your Birthday")]
        public DateTime? Birthday { get; set; }

        public bool? RememberMe { get; set; }

        public string? ProfileImage { get; set; }

    }
}