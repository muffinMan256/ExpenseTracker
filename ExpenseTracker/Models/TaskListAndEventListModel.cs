using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenseTracker.Data;

namespace ExpenseTracker.Models
{
        public class TaskModel
        {
            [Key]
            public int ListId { get; set; }

            [Required(ErrorMessage = "This field is mandatory!")]
            public string TaskName { get; set; }

            [Required(ErrorMessage = "This field is mandatory!")]
            public DateTime TaskDate { get; set; } = DateTime.Now;

            [Required(ErrorMessage = "This field is mandatory!")]
            public string TaskPriority { get; set; }

            [Required(ErrorMessage = "This field is mandatory!")]
            public bool TaskStatus { get; set; }

            public string? UserId { get; set; }
        }


        public class EventModel
        {
            [Key]
            public int EventId { get; set; }


            [Required(ErrorMessage = "This field is mandatory!")]
            public string EventName { get; set; }


            public string? UserId { get; set; }
    }
        


    }
