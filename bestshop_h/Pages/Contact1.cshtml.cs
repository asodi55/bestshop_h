using bestshop_h.myhelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Data.SqlClient;

namespace bestshop_h.Pages
{
    public class Contact1Model : PageModel
    {
        public void OnGet()
        {
        }

        [BindProperty]
        [Required(ErrorMessage = "the First Name is Required")]
        [Display(Name = "First Name*")]
        public string FirstName { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "the Last Name is Required")]
        [Display(Name = "Last Name*")]
        public string LastName { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "the Email is Required")]
        [EmailAddress]
        [Display(Name = "Email*")]
        public string Email { get; set; } = "";

        [BindProperty]
        public string? Phone { get; set; } = "";

        [BindProperty, Required]
        [Display(Name = "Subject*")]
        public string Subject { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "the Message is Required")]
        [MinLength(5, ErrorMessage = "the message should be at least 5 characters")]
        [MaxLength(1024, ErrorMessage = "the message should be at least than 1024 characters")]
        [Display(Name = "Message*")]
        public string Message { get; set; } = "";

        public List<SelectListItem> Subjectlist { get; } = new List<SelectListItem>
        {
            new SelectListItem{ Value = "Oder status", Text = "Oder status" },
            new SelectListItem{ Value = "Refund request", Text = "Refund request" },
            new SelectListItem{ Value = "Job Application", Text = "Job Application" },
            new SelectListItem{ Value = "Other", Text = "Other" },
        };
        public string SuccessMessage { get; set; } = "";
        public string ErrorMessage { get; set; } = "";

        public void OnPost()
        {


            // check if any required field is empty
            if (!ModelState.IsValid)
            {
                //Error
                ErrorMessage = "please fill all required fields";
                return;
            }
            if (Phone == null) Phone = "";
            // Add this message to the database
            try
            {
                string connectionstring = "Data Source=mssqluk22.prosql.net;Initial Catalog=cmsapps;Persist Security Info=True;User ID=emp;Password=inDia@143";
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();
                    string sql = "INSERT INTO messages_h " +
                        "(firstname, lastname, email, phone, subject, message) VALUES " +
                        "(@firstname, @lastname, @email, @phone, @subject, @message);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@firstname", FirstName);
                        command.Parameters.AddWithValue("@lastname", LastName);
                        command.Parameters.AddWithValue("@email", Email);
                        command.Parameters.AddWithValue("@phone", Phone);
                        command.Parameters.AddWithValue("@subject", Subject);
                        command.Parameters.AddWithValue("@message", Message);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                //Error
                ErrorMessage = "please fill all required fields";
                return;
            }

            // send confirmation Emai-l to the client
            //string username = FirstName + " " + LastName;

            //string emailSubject = "About your message";
            //string emailMessage = "Dear"
            //+ username + ",\n" + "we recevied your message. thank you for contacting us.\n" + "Our team will contact you very soon.\n" + "best Regars\n\n" + "your Message:\n" + Message;

            //Emailsender.SendEmail(Email, username, emailSubject, emailMessage).Wait();

            SuccessMessage = "your message has been received correctly";

            FirstName = "";
            LastName = "";
            Email = "";
            Phone = "";
            Subject = "";
            Message = "";

            ModelState.Clear();
        }
    }
}
