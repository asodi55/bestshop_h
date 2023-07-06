using bestshop_h.myhelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace bestshop_h.Pages.Admin.Books
{
    [Requireauthattribute(RequiredRole = "admin")]
    public class EditModel : PageModel
    {
        [BindProperty]
        public int id { get; set; }


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
        public string image_filename { get; set; } = "";

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public string errorMessage = "";
        public string successMessage = "";

        private IWebHostEnvironment webHostEnvironment;

        public EditModel(IWebHostEnvironment env)
        {
            webHostEnvironment = env;
        }

        public void OnGet()
        {
            string requestId = Request.Query["id"];

            try
            {
                string connectionstring = "Data Source=mssqluk22.prosql.net;Initial Catalog=cmsapps;Persist Security Info=True;User ID=emp;Password=inDia@143";
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();
                    string sql = "SELECT * FROM books_h WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", requestId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                id = reader.GetInt32(0);
                                title = reader.GetString(1);
                                authors = reader.GetString(2);
                                isbn = reader.GetString(3);
                                num_pages = reader.GetInt32(4);
                                price = reader.GetDecimal(5);
                                category = reader.GetString(6);
                                description = reader.GetString(7);
                                image_filename = reader.GetString(8);
                            }
                            else
                            {
                                Response.Redirect("/Admin/Books/Index");
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                successMessage = "Data saved correctly";
                Response.Redirect("/Admin/Books/Index");
            }
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

            // if we have a new iagefile => upload the new image and delete the old image
            string newFileName = image_filename;
            if (ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(ImageFile.FileName);

                string imageFolder = webHostEnvironment.WebRootPath + "/img/books/";
                string imageFullPath = Path.Combine(imageFolder, newFileName);
                Console.WriteLine("New image (Edit): " + imageFullPath);

                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    ImageFile.CopyTo(stream);
                }

                // delete old image
                string oldImageFullPath = Path.Combine(imageFolder, image_filename);
                System.IO.File.Delete(oldImageFullPath);
                Console.WriteLine("Delete Image " + oldImageFullPath);
            }

            // update the book data in the database
            try
            {
                string connectionstring = "Data Source=mssqluk22.prosql.net;Initial Catalog=cmsapps;Persist Security Info=True;User ID=emp;Password=inDia@143";
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();
                    string sql = "UPDATE books_h SET title=@title, authors=@authors, isbn=@isbn, " +
                        "num_pages=@num_pages, price=@price, category=@category, " +
                        "description=@description, image_filename=@image_filename WHERE id=@id;";

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
                        command.Parameters.AddWithValue("@id", id);

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
