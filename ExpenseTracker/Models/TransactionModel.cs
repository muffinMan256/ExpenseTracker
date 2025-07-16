using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Models
{
    public class TransactionModel
    {
        [Key]
        public int TransactionId { get; set; }

        //FK
        [Range(1, int.MaxValue, ErrorMessage = "Please select a category!")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "This field is mandatory!")]
        [Range(1, int.MaxValue, ErrorMessage = "Amount should be greater than 0!")]
        public int Amount { get; set; }

        [Required(ErrorMessage = "This field is mandatory!")]
        [Column(TypeName = "nvarchar(75)")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "This field is mandatory!")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "This field is mandatory!")]
        public CategoryModel? Category {get; set; }

        [Required]
        [Column(TypeName = "nvarchar(450)")]
        public string UserId { get; set; }

    }
}
