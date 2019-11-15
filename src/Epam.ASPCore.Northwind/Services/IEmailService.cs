using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Epam.ASPCore.Northwind.WebUI.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
