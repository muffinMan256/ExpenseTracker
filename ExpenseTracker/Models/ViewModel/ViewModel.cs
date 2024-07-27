using ExpenseTracker.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models.ViewModel
{
    public class ViewModelCatTrans
    {
        public List<ExpenseTracker.Models.CategoryModel>? CategoriesList { get; set; }
        public List<Transaction>? TransactionsList { get; set; }

        public CategoryModel? Categories { get; set; }

        public TransactionModel? Transactions { get; set; }

    }

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
    }


}
