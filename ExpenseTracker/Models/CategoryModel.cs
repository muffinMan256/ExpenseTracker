using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class CategoryModel
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "This field is mandatory!")]
        [StringLength(20)]
        [Display(Name = "Name your Expense Category", Prompt = "titlu categorie")]
        public string Title { get; set; }

        [Required(ErrorMessage = "This field is mandatory!")]
        [Display(Name = "Pick your Icon")]
        public string Icon { get; set; } = "";

        [Required(ErrorMessage = "This field is mandatory!")]
        [Display(Name = "Expense Type")]
        public string Type { get; set; } = "";

        [Required(ErrorMessage = "This field is mandatory!")]
        [Display(Name = "Date of creation")]
        public DateTime? CreationDate { get; set; } = DateTime.Now;

        [Display(Name = "Note")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "This field is mandatory!")]
        [Display(Name = "Priority")]
        public string? Priority { get; set; }

        [Required(ErrorMessage = "This field is mandatory!")]
        [Display(Name = "Recurring")]
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