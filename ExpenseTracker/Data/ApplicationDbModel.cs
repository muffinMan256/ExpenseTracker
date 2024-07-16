using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Data
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? Birthday { get; set; }

    }
    public class Category
    {

        [Key]
        public int CategoryId { get; set; }

        public string Title { get; set; }

        public string Icon { get; set; } = "";

        public string Type { get; set; }

        public DateTime? CreationDate { get; set; } = DateTime.Now;

        public string? Note { get; set; }

        public string? Priority { get; set; }

        public string? Recurring { get; set; }

        [NotMapped]
        public string TitleWithIcon => Icon + " " + Title;
    }
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        public int CategoryId { get; set; }

        public int Amount { get; set; }

        public string? Note { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public string? CategoryTitleWithIcon => Category == null ? "" : Category.Icon + " " + Category.Title;

        [NotMapped]
        public string? FormattedAmount => (Category == null || Category.Type == "Expense" ? "- " : "+ ") + Amount.ToString("C0");

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}