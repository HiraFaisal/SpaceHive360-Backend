using System.Threading.Tasks;

namespace SpaceHive360.Admin.Application.Services.Emails
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
