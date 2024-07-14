using ExpenseTracker.Models;

namespace ExpenseTracker.ViewModel
{
    public class CategoryTransactionCombinedViewModel
    {
        public Transaction Transactions { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
