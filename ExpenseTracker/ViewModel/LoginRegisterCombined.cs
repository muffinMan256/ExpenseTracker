using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace ExpenseTracker.ViewModel
{
    public class LoginRegisterCombined
    {
        public LoginModel LoginModel { get; set; }

        public RegisterModel RegisterModel { get; set; }
    }
}
