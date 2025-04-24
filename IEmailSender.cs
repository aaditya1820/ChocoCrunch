using Microsoft.AspNetCore.Mvc;

namespace Aaditya.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
