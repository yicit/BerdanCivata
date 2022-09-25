using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;



namespace BerdanCivata.Pages.Clients
{
    public class CreateModel : PageModel
    {
        public readonly IHostEnvironment environment;

        public CreateModel(IHostEnvironment environment)
        {
            this.environment = environment;
        }

        public ClientInfo clientinfo = new ClientInfo();
        public String errorMessage="";
        public String succesMessage = "";

        public void OnGet()
        {
        }

        public void OnPost(IFormFile file)
        {
            var filepath = Path.Combine(environment.ContentRootPath, "images", "default.png");
            if (file != null)
            {
                filepath = Path.Combine(environment.ContentRootPath, "images", file.FileName);

            }

            //Get the all the from web page
            clientinfo.name = Request.Form["name"];
            clientinfo.email = Request.Form["email"];
            clientinfo.phone = Request.Form["phone"];
            clientinfo.address = Request.Form["address"];
           
      
            //Check if there a missing parameter
            if(clientinfo.name.Length ==0 || clientinfo.email.Length ==0 || clientinfo.phone.Length==0 || clientinfo.address.Length == 0)
            {
                errorMessage = "All the fields are required";
                return;
            }
             
            //To save the client into database
            try
            {
                String connectionString = "Data Source=.\\sqlexpress;Initial Catalog=berdan;Integrated Security=True";
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //ýf user upload an image it is saved in db with binary form 
                    //else default image is added as a image of user
                    String sql = "INSERT INTO clients" + "(name,email,phone,address,imageCol) VALUES" +
                        "(@name,@email,@phone,@address," + "(Select BulkColumn FROM Openrowset(Bulk '" + filepath + "',Single_Blob) as img))";
                        

                    //save the new client into db
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", clientinfo.name);
                        command.Parameters.AddWithValue("@email", clientinfo.email);
                        command.Parameters.AddWithValue("@phone", clientinfo.phone);
                        command.Parameters.AddWithValue("@address", clientinfo.address);
                        command.Parameters.AddWithValue("@imageCol", clientinfo.address);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage=ex.Message;
                return;
            }

            clientinfo.name = "";
            clientinfo.email = "";
            clientinfo.phone = "";
            clientinfo.address = "";

            //If insertion is succesful 
            succesMessage = "New Client Added!!!";
            Response.Redirect("/Clients");

        }
    }
}
