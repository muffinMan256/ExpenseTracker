using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace ExpenseTracker.Models
{
    public class ApplicationDbContext:DbContext
    {
        //constructor - add the correspondent dbConnction string
        public ApplicationDbContext(DbContextOptions options):base(options)
        {

        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Register> Registers { get; set; }
        public DbSet<Login> Logins { get; set; }



    }
}
