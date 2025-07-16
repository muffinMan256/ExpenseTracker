using ExpenseTracker.Data;
using ExpenseTracker.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Models.ViewModel
{

    public class ViewModelCatTrans
    {

        public class PaginatedList<T> : List<T>
        {
            public int PageIndex { get; private set; }
            public int TotalPages { get; private set; }

            public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
            {
                PageIndex = pageIndex;
                TotalPages = (int)Math.Ceiling(count / (double)pageSize);

                this.AddRange(items);
            }

            public bool HasPreviousPage => PageIndex > 1;

            public bool HasNextPage => PageIndex < TotalPages;

            public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
            {
                var count = await source.CountAsync();
                var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                return new PaginatedList<T>(items, count, pageIndex, pageSize);
            }
        }

        public List<EventModel>? EventModelList { get; set; }
        public List<TaskModel>? TaskModelList { get; set; }

        public TaskModel? Tasks { get; set; }

        public EventModel? Events { get; set; }

        public List<ExpenseTracker.Models.CategoryModel>? CategoriesList { get; set; }
        public List<TransactionModel>? TransactionsList { get; set; }
        public List<TransactionModel>? TransactionModelList { get; set; }

        public CategoryModel? Categories { get; set; }

        public TransactionModel? Transactions { get; set; }

        private string? userId;
        
        public string UserId
        {
            get => Transactions?.UserId ?? userId;
            set
            {
                userId = value;
                if (Transactions != null)
                {
                    Transactions.UserId = value;
                }
            }
        }

        public List<AppUser> Users { get; set; }

        public ViewModelCatTrans()
        {
            Users = new List<AppUser>();
        }

        [NotMapped]
        public decimal TotalExpenses { get; set; }
        [NotMapped]
        public decimal TotalIncome { get; set; }
        [NotMapped]
        public decimal Balance { get; set; }

        [NotMapped]
        public decimal Savings { get; set; }



    }
    //public class TransactionModel
    //{
    //    [Key]
    //    public int TransactionId { get; set; }

    //    //FK
    //    [Range(1, int.MaxValue, ErrorMessage = "Please select a category!")]
    //    public int CategoryId { get; set; }

    //    [Required(ErrorMessage = "This field is mandatory!")]
    //    [Range(1, int.MaxValue, ErrorMessage = "Amount should be greater than 0!")]
    //    public int Amount { get; set; }

    //    [Required(ErrorMessage = "This field is mandatory!")]
    //    [Column(TypeName = "nvarchar(75)")]
    //    public string? Note { get; set; }

    //    [Required(ErrorMessage = "This field is mandatory!")]
    //    public DateTime Date { get; set; } = DateTime.Now;

    //    public string UserId { get; set; }

    //}
}
