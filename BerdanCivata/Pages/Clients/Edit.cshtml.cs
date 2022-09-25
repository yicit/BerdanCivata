using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BerdanCivata.Pages.Clients
{
    public class EditModel : PageModel
    {
        public ClientInfo clientInfo = new ClientInfo();
        public String errorMessage = "";
        public String succesMessage = "";
        public readonly IHostEnvironment environment;

        public EditModel(IHostEnvironment environment)
        {
            this.environment = environment;
        }

        public void OnGet()
        {
            String id = Request.Query["id"];

            try
            {
                String connectionString = "Data Source=.\\sqlexpress;Initial Catalog=berdan;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //To get desired client from db
                    String sql = " SELECT * FROM clients WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                clientInfo.id = "" + reader.GetInt32(0);
                                clientInfo.name = reader.GetString(1);
                                clientInfo.email = reader.GetString(2);
                                clientInfo.phone = reader.GetString(3);
                                clientInfo.address = reader.GetString(4);
                                clientInfo.imageCol = (byte[])reader[6];

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        //To get the image path is get the file from web page
        public void OnPost(IFormFile file)
        {

            //Get the all the from web page
            clientInfo.id = Request.Form["id"];
            clientInfo.name = Request.Form["name"];
            clientInfo.email = Request.Form["email"];
            clientInfo.phone = Request.Form["phone"];
            clientInfo.address = Request.Form["address"];


            //Check if there a missing parameter
            if (clientInfo.id.Length == 0 ||  clientInfo.name.Length == 0 || clientInfo.email.Length == 0 || clientInfo.phone.Length == 0 || clientInfo.address.Length == 0)
            {
                errorMessage = "All the fields are required";
                return;
            }


            //To update the client into database
            try
            {
                String connectionString = "Data Source=.\\sqlexpress;Initial Catalog=berdan;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "";
                    //Check there is a new user image or not if there is update the image
                    if (file != null)
                    {
                        var filepath = Path.Combine(environment.ContentRootPath, "images", file.FileName);
                        sql= "UPDATE clients " +
                        "SET name=@name, email=@email, phone=@phone, address=@address,imageCol=(SELECT BulkColumn FROM Openrowset( Bulk '"+filepath+"', Single_Blob) as img)  WHERE id=@id";
                    }
                    else
                    {
                         sql = "UPDATE clients " +
                        "SET name=@name, email=@email, phone=@phone, address=@address WHERE id=@id";
                    }

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        //The data received from the web page 
                        command.Parameters.AddWithValue("@name", clientInfo.name);
                        command.Parameters.AddWithValue("@email", clientInfo.email);
                        command.Parameters.AddWithValue("@phone", clientInfo.phone);
                        command.Parameters.AddWithValue("@address", clientInfo.address);
                        command.Parameters.AddWithValue("@id", clientInfo.id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
            //If data is updated correctly redirect to user
            Response.Redirect("/Clients");
        }
    }
}
