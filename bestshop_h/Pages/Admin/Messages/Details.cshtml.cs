using bestshop_h.myhelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace bestshop_h.Pages.Admin.Messages
{
    [Requireauthattribute(RequiredRole = "admin")]
    public class DetailsModel : PageModel
    {
        public MessageInfo messageInfo = new MessageInfo();
        public void OnGet()
        {
            string requestId = Request.Query["id"];
            try
            {
                string connectionstring = "Data Source=mssqluk22.prosql.net;Initial Catalog=cmsapps;Persist Security Info=True;User ID=emp;Password=inDia@143";
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();
                    string sqlCount = "SELECT * FROM messages_h WHERE id=@id";

                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        command.Parameters.AddWithValue("@id", requestId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                messageInfo.Id = reader.GetInt32(0);
                                messageInfo.FirstnName = reader.GetString(1);
                                messageInfo.LastnName = reader.GetString(2);
                                messageInfo.Email = reader.GetString(3);
                                messageInfo.Phone = reader.GetString(4);
                                messageInfo.Subject = reader.GetString(5);
                                messageInfo.Message = reader.GetString(6);
                                messageInfo.CreatedAt = reader.GetDateTime(7).ToString("MM/dd/yyyy");

                            }
                            else
                            {
                                Response.Redirect("/Admin/Messages/Index");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response.Redirect("/Admin/Messages/Index");
            }
        }
    }
}
