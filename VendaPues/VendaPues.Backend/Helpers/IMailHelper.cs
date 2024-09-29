using VendaPues.Shared.Responses;

namespace VendaPues.Backend.Helpers
{
    public interface IMailHelper
    {
        ActionResponse<string> SendMail(string toName, string toEmail, string subject, string body);
    }
}