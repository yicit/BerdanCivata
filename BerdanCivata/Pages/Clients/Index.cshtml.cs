using System.Data;
using System.Data.SqlClient;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Net.Mime.MediaTypeNames;

namespace BerdanCivata.Pages.Clients
{
    public class IndexModel : PageModel
    {

        //Client listesi yaratma
        public List<ClientInfo> listCliens = new List<ClientInfo>();
        
        public void OnGet()
        {
           try
            {

                //Database connection string
                String connectionString = "Data Source=.\\sqlexpress;Initial Catalog=berdan;Integrated Security=True";


                using(SqlConnection connection = new SqlConnection(connectionString))
                {

                    //open connection and create sql query to read from clients table 
                    connection.Open();
                    String sql = "Select * FROM clients";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            //Read data from clients table
                            while (reader.Read())
                            {
                                ClientInfo clientInfo = new ClientInfo();
                                clientInfo.id = "" + reader.GetInt32(0);
                                clientInfo.name = reader.GetString(1);
                                clientInfo.email = reader.GetString(2);
                                clientInfo.phone = reader.GetString(3);
                                clientInfo.address = reader.GetString(4);
                                clientInfo.created_at = reader.GetDateTime(5).ToString();
                                clientInfo.imageCol = (byte[])reader[6];

                                //Save clients to list
                                listCliens.Add(clientInfo);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception: "+ ex.ToString());
            }
        }
    }


    //To store data in database
    public class ClientInfo
    {
        public String id;
        public String name;
        public String email;
        public String phone;
        public String address;
        public String created_at;
        public byte[] imageCol;
    }
}
