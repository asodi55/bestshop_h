using bestshop_h.myhelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace bestshop_h.Pages.Auth
{
    [Requirenouthattribute]
    public class ResetpasswordModel : PageModel
    {
        [BindProperty, Required(ErrorMessage = "Password is required")]
        [StringLength(50, ErrorMessage = "Password must be between 5 and 50 characters", MinimumLength = 5)]
        public string Password { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; } = "";

        public string errorMessage = "";
        public string successMessage = "";
        public void OnGet()
        {
            string token = Request.Query["token"];
            if (string.IsNullOrEmpty(token))
            {
                Response.Redirect("/");
                return;
            }


        }
        public void OnPost()
        {
            string token = Request.Query["token"];
            if (string.IsNullOrEmpty(token))
            {
                Response.Redirect("/");
                return;
            }

            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }

            // connect to database and update the password
            try
            {
                string connectionstring = "Data Source=mssqluk22.prosql.net;Initial Catalog=cmsapps;Persist Security Info=True;User ID=emp;Password=inDia@143";
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();

                    // 1) check if the token is valid, and get the user email address from password_resets_h
                    string email = "";
                    string sql = "SELECT * FROM password_resets_h WHERE token=@token";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@token", token);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                email = reader.GetString(0);
                            }
                            else
                            {
                                errorMessage = "Wrong or Expired Token";
                            }
                        }
                    }

                    // 2) encrypt the new password and update the user password in users table
                    var passwordHasher = new PasswordHasher<IdentityUser>();
                    string hashedPassword = passwordHasher.HashPassword(new IdentityUser(), Password);

                    sql = "UPDATE users_h SET password=@password WHERE email=@email";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@password", hashedPassword);
                        command.Parameters.AddWithValue("@email", email);

                        command.ExecuteNonQuery();
                    }

                    // 3) delete the token from the database (password_resets table)
                    sql = "DELETE FROM password_resets_h WHERE email=@email";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", email);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            successMessage = "Password reset successfully";
        }
    }
}
