using bestshop_h.myhelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace bestshop_h.Pages.Admin.Books
{
    [Requireauthattribute(RequiredRole = "admin")]
    public class IndexModel : PageModel
    {
        public List<Booksinfo> listBooks = new List<Booksinfo>();
        public string search = "";

        public int page = 1; // the current html page
        public int totalPages = 0;
        private readonly int pageSize = 2; // books per page


        public string column = "id";
        public string order = "desc";
        public void OnGet()
        {
            search = Request.Query["search"];
            if (search == null) search = "";

            page = 1;
            string requestPage = Request.Query["page"];
            if (requestPage != null)
            {
                try
                {
                    page = int.Parse(requestPage);
                }
                catch (Exception ex)
                {
                    page = 1;
                }
            }

            string[] validColumns = { "id", "title", "authors", "num_pages", "price", "category", "created_at" };
            column = Request.Query["column"];
            if (column == null || !validColumns.Contains(column))
            {
                column = "id";
            }

            order = Request.Query["order"];
            if (order == null || !order.Equals("asc"))
            {
                order = "desc";
            }

            try
            {
                string connectionstring = "Data Source=mssqluk22.prosql.net;Initial Catalog=cmsapps;Persist Security Info=True;User ID=emp;Password=inDia@143";
                using (SqlConnection connection = new SqlConnection(connectionstring))

                {
                    connection.Open();
                    string sqlCount = "SELECT COUNT(*) FROM books_h";
                    if (search.Length > 0)
                    {
                        sqlCount += " WHERE title LIKE @search OR authors LIKE @search";
                    }
                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        command.Parameters.AddWithValue("@search", "%" + search + "%");

                        decimal count = (int)command.ExecuteScalar();
                        totalPages = (int)Math.Ceiling(count / pageSize);
                    }


                    string sql = "SELECT * FROM books_h";
                    if (search.Length > 0)
                    {
                        sql += " WHERE title LIKE @search OR authors LIKE @search";
                    }
                    sql += " ORDER BY " + column + " " + order; //" ORDER BY id DESC";
                    sql += " OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@search", "%" + search + "%");
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);


                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Booksinfo booksinfo = new Booksinfo();
                                booksinfo.id = reader.GetInt32(0);
                                booksinfo.title = reader.GetString(1);
                                booksinfo.authors = reader.GetString(2);
                                booksinfo.isbn = reader.GetString(3);
                                booksinfo.num_pages = reader.GetInt32(4);
                                booksinfo.price = reader.GetDecimal(5);
                                booksinfo.category = reader.GetString(6);
                                booksinfo.description = reader.GetString(7);
                                booksinfo.image_filename = reader.GetString(8);
                                booksinfo.created_at = reader.GetDateTime(9).ToString("MM/dd/yyyy");

                                listBooks.Add(booksinfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class Booksinfo
    {
        public int id { get; set; }
        public string title { get; set; } = "";
        public string authors { get; set; } = "";
        public string isbn { get; set; } = "";
        public int num_pages { get; set; }
        public decimal price { get; set; }
        public string category { get; set; } = "";
        public string description { get; set; } = "";
        public string image_filename { get; set; } = "";
        public string created_at { get; set; } = "";

    }
}