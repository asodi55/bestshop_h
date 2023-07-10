using bestshop_h.myhelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;

namespace bestshop_h.Pages.Auth
{
    //[Requirenouthattribute]
    public class ForgotpasswordModel : PageModel
    {

        [BindProperty, Required(ErrorMessage = "The Email is required"), EmailAddress]
        public string Email { get; set; } = "";

        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }
        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }

            // 1) create token, 2) save token in the database, 3) send token by email to the user
            try
            {
                

                string connectionstring = "Data Source=mssqluk22.prosql.net;Initial Catalog=cmsapps;Persist Security Info=True;User ID=emp;Password=inDia@143";
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();
                    string sql = "SELECT * FROM users_h WHERE email=@email";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", Email);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string firstname = reader.GetString(1);
                                string lastname = reader.GetString(2);

                                string token = Guid.NewGuid().ToString();

                                // save the token in the database
                                SaveToken(Email, token);

                                // send the token by email to the user
                                string resetUrl = Url.PageLink("/Auth/Resetpassword") + "?token=" + token;
                                string username = firstname + " " + lastname;
                                string subject = "Password Reset";
                                string message = "Dear " + username + ",\n\n" +
                                    "You can reset your password using the following link:\n\n" +
                                    resetUrl + "\n\n" +
                                    "Best Regards";

                                //Emailsender.Sendemail(Email, username, subject, message).Wait();
                                MailMessage semail = new MailMessage();
                                SmtpClient smtp = new SmtpClient();
                                semail.From = new MailAddress("noreply@cabletvcrm.net", "Cable TV CRM");
                                semail.To.Add(Email);
                                //semail.CC.Add("k.likhitha5@gmail.com");
                                //semail.CC.Add("susmithavari@gmail.com");
                                semail.Subject = "BestShop";
                                semail.IsBodyHtml = true;
                                semail.Body = resetUrl + "?token=" + token;
                                smtp.Port = 366;
                                smtp.Host = "mailuk2.promailserver.com";
                                smtp.EnableSsl = false;
                                smtp.UseDefaultCredentials = false;
                                smtp.Credentials = new NetworkCredential("noreply@cabletvcrm.net", "Bang@2205$");
                                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                                smtp.Send(semail);

                            }
                            else
                            {
                                errorMessage = "We have no user with this email address";
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
            successMessage = "Please check your email and click on the reset password link";
        }
        private void SaveToken(string email, string token)
        {
            try
            {
                DateTime createutctime = DateTime.UtcNow;
                TimeZoneInfo createmyist = TimeZoneInfo.CreateCustomTimeZone("Bangalore", new TimeSpan(+5, 30, 0), "Bangalore", "Bangalore");
                DateTime createdatetime = TimeZoneInfo.ConvertTimeFromUtc(createutctime, createmyist);

                string connectionstring = "Data Source=mssqluk22.prosql.net;Initial Catalog=cmsapps;Persist Security Info=True;User ID=emp;Password=inDia@143";
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();

                    // delete any old token for this email address from the database
                    string sql = "DELETE FROM password_resets_h WHERE email=@email";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);

                        command.ExecuteNonQuery();
                    }

                    // add token to database
                    sql = "INSERT INTO password_resets_h (email, token) VALUES (@email, @token)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@token", token);
                        command.Parameters.AddWithValue("created_at", createdatetime);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
