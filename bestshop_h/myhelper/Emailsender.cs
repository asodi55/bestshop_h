using SendGrid;
using SendGrid.Helpers.Mail;

namespace bestshop_h.myhelper
{
    public class Emailsender
    {
        public static void SendEmail(string toEmail, string username, string subject, string message)
        {
            //string apikey = ""
            //    var client = new SendGridClient(apikey);

            //var from = new EmailAddress(username"boostmytoo@gmail.com", "BestShop.com");
            //var to = new EmailAddress(toEmail, username);
            //var plainTextContent = message;
            //var htmlContent = "";

            //var msg = MailHelper.CreateSingleEmail(from, to subject, plainTextContent, htmlContent);

            //var response = client.SendEmailAsync(msg);
        }
    }
}
