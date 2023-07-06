using bestshop_h.myhelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

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
            successMessage = "Password reset successfully";
        }
    }
}
