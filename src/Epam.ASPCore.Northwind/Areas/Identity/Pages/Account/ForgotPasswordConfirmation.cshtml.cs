using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Epam.ASPCore.Northwind.WebUI.Areas.Identity.Pages.Account
{
    public class ForgotPasswordConfirmation : PageModel
    {
        [BindProperty]
        public string Message { get; set; }

        public void OnGet(bool success)
        {
            Message = success ? "Please check your email to reset your password." : "Sorry there was an error when sending emails!";
        }
    }
}