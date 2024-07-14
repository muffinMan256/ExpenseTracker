﻿using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace ExpenseTracker.ViewModel
{
    public class CategoryTransactionCombinedViewModel
    {
        public Transaction Transactions { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }

    public class LoginRegisterViewModel
    {
        public LoginModel LoginModel { get; set; }

        public RegisterModel RegisterModel { get; set; }
    }
}
