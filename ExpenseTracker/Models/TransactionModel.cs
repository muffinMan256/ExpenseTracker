using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenseTracker.Data;

namespace ExpenseTracker.Models
{
    public class TransactionModel
    {
        [Key]
        public int TransactionId { get; set; }

        //FK
        [Range(1, int.MaxValue, ErrorMessage = "Please select a category!")]
        public int CategoryId { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Amount should be greater than 0!")]
        public int Amount { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public string? Note { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public string? CategoryTitleWithIcon { get; set; }

        [NotMapped]
        public string? FormattedAmount { get; set; }

        public Category Category {get; set; }
    }
}
