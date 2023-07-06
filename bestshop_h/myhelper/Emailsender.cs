using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using System.Net;
using System.Numerics;

namespace bestshop_h.myhelper
{
    public class Emailsender
    {
        public void Sendemail()
        {
            MailMessage semail = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            semail.From = new MailAddress("noreply@cabletvcrm.net", "Cable TV CRM");
            semail.To.Add("");
            //semail.CC.Add("k.likhitha5@gmail.com");
            //semail.CC.Add("susmithavari@gmail.com");
            semail.Subject = "BestShop";
            semail.IsBodyHtml = true;
            semail.Body = "";
            smtp.Port = 366;
            smtp.Host = "mailuk2.promailserver.com";
            smtp.EnableSsl = false;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("noreply@cabletvcrm.net", "Bang@2205$");
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(semail);
        }
    }
}
