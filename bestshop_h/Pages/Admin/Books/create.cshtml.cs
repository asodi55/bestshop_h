using bestshop_h.myhelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace bestshop_h.Pages.Books
{
    [Requireauthattribute(RequiredRole = "admin")]
    public class createModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "The title is required")]
        [MaxLength(100, ErrorMessage = "The title cannot exceed 100 characters")]
        public string title { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "The author is required")]
        [MaxLength(255, ErrorMessage = "The authors cannot exceed 255 characters")]
        public string authors { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "The isbn is required")]
        [MaxLength(20, ErrorMessage = "The isbn cannot exceed 20 characters")]
        public string isbn { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "The num_pages of Pages is required")]
        [Range(1, 10000, ErrorMessage = "The num_pages of Pages must be in the range from 1 to 10000")]
        public int num_pages { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "The price is required")]
        public decimal price { get; set; }

        [BindProperty, Required]
        public string category { get; set; } = "";

        [BindProperty]
        [MaxLength(1000, ErrorMessage = "The description cannot exceed 1000 characters")]
        public string? description { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "The image_filename File is required")]
        public IFormFile image_filename { get; set; }

        public string errorMessage = "";
        public string successMessage = "";

        private IWebHostEnvironment webHostEnvironment;

        public createModel(IWebHostEnvironment env)
        {
            webHostEnvironment = env;
        }



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

            // successfull data validation

            if (description == null) description = "";

            // save the image file on the server
            // 
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(image_filename.FileName);

            string imageFolder = webHostEnvironment.WebRootPath + "/img/books/";

            string imageFullPath = Path.Combine(imageFolder, newFileName);
            Console.WriteLine("New image: " + imageFullPath);

            using (var stream = System.IO.File.Create(imageFullPath))
            {
                image_filename.CopyTo(stream);
            }


            // save the new book in the database
            // 
            try
            {
                string connectionstring = "Data Source=mssqluk22.prosql.net;Initial Catalog=cmsapps;Persist Security Info=True;User ID=emp;Password=inDia@143";
                using (SqlConnection connection = new SqlConnection(connectionstring))


                {
                    connection.Open();
                    string sql = "INSERT INTO books_h " +
                    "(title, authors, isbn, num_pages, price, category, description, image_filename) VALUES " +
                    "(@title, @authors, @isbn, @num_pages, @price, @category, @description, @image_filename);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@title", title);
                        command.Parameters.AddWithValue("@authors", authors);
                        command.Parameters.AddWithValue("@isbn", isbn);
                        command.Parameters.AddWithValue("@num_pages", num_pages);
                        command.Parameters.AddWithValue("@price", price);
                        command.Parameters.AddWithValue("@category", category);
                        command.Parameters.AddWithValue("@description", description);
                        command.Parameters.AddWithValue("@image_filename", newFileName);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            successMessage = "Data saved correctly";
            Response.Redirect("/Admin/Books/Index");

        }
    }
}
