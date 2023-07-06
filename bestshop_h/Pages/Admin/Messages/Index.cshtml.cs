using bestshop_h.myhelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace bestshop_h.Pages.Admin.Messages
{
    [Requireauthattribute(RequiredRole = "admin")]
    public class IndexModel : PageModel
    {
        public List<MessageInfo> listMessages = new List<MessageInfo>();
        public int page = 1; //the current html page
        public int totalpages = 0;
        private readonly int pageSize = 5; // each html page shows pageSize message
        public void OnGet()
        {
            page = 1;
            string requestPage = Request.Query["page"];
            if (requestPage != null)
            {
                try
                {
                    page = int.Parse(requestPage);
                }
                catch(Exception ex)
                {
                    page = 1;
                }
            }
            try
            {
                string connectionstring = "Data Source=mssqluk22.prosql.net;Initial Catalog=cmsapps;Persist Security Info=True;User ID=emp;Password=inDia@143";
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    connection.Open();

                    string sqlCount = "SELECT COUNT(*) FROM messages_h";
                    using (SqlCommand command = new SqlCommand(sqlCount, connection))
                    {
                        decimal count = (int)command.ExecuteScalar();
                        totalpages = (int)Math.Ceiling(count / pageSize);
                    }
                        string sql = "SELECT * FROM messages_h ORDER BY id DESC";
                    sql += " OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MessageInfo messageInfo = new MessageInfo();
                                messageInfo.Id = reader.GetInt32(0);
                                messageInfo.FirstnName = reader.GetString(1);
                                messageInfo.LastnName = reader.GetString(2);
                                messageInfo.Email = reader.GetString(3);
                                messageInfo.Phone = reader.GetString(4);
                                messageInfo.Subject = reader.GetString(5);
                                messageInfo.Message = reader.GetString(6);
                                messageInfo.CreatedAt = reader.GetDateTime(7).ToString("MM/dd/yyyy");

                                listMessages.Add(messageInfo);
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

    public class MessageInfo
    {
        public int Id { get; set; }
        public string FirstnName { get; set; } = "";
        public string LastnName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Message { get; set; } = "";
        public string CreatedAt { get; set; } = "";


    }
}

