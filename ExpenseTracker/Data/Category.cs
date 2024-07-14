using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Data
{
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
        public string TitleWithIcon
        {
            get
            {
                return Icon + " " + Title;
            }
        }
    }
}
