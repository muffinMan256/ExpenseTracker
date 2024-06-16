using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Models;

// Add profile data for application users by adding properties to the ExpenseTrackerUser class
public class ExpenseTrackerUser : IdentityUser
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime? Birthday { get; set; }
}

