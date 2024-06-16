using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Column(TypeName = "nvarchar(50)")] public string FirstName { get; set; }

        [Column(TypeName = "nvarchar(50)")] public string LastName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")] public string Email { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")] public string Password { get; set; }
    }
}
