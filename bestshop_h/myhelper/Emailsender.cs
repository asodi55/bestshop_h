using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using System.Numerics;

namespace bestshop_h.myhelper
{
    public class Emailsender
    {
        public static object? Email { get; private set; }
        public static object? FirstName { get; private set; }
        public static object? LastName { get; private set; }
        public static object? Phone { get; private set; }

        public static async Task SendEmail(string toEmail, string username, string subject, string message)
        {
            MailMessage Msg = new MailMessage();
            Msg.From = new MailAddress("contact@winnovative.in", "TestEmail");
            Msg.To.Add("Email");
            Msg.CC.Add("susmithavari@gmail.com");
            Msg.CC.Add("srihariasodi2@gmail.com");
            Msg.Subject = "Test Email";
            Msg.Body = "<p>Name:" + FirstName + "</p>" + "<p>Name:" + LastName + "</p>" + "<p>E-mail: " + Email + "</p>" + "<p>Phone:" + Phone + "</p>";
            Msg.IsBodyHtml = true;



            SmtpClient smtp = new SmtpClient();
            smtp.Host = "relay-hosting.secureserver.net";
            smtp.Port = 25;
            smtp.Credentials = new System.Net.NetworkCredential("contact@winnovative.in", "crossRef@040$");
            smtp.EnableSsl = false;
            smtp.Send(Msg);
            Msg = null;
        }
    }
}
