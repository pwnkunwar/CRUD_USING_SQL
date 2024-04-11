using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using WEBAPI.Models;

namespace WEBAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    //[Route("api/[controller]/[action]")]
    public class CategoryController : ControllerBase
    {
       
        private readonly IConfiguration _configuration;
        public CategoryController(IConfiguration configuration)
        {
            _configuration= configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                 List<Category> categories = new List<Category>();
                 string connectionString = _configuration.GetConnectionString("Database");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("selectData",connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var category = new Category
                                    {
                                        Id = reader.GetInt32("Id"),
                                        Name = reader.GetString("Name"),
                                        Email = reader.GetString("Email"),
                                        Phone = reader.GetString("Phone"),
                                        Address = reader.GetString("Address")

                                    };
                                   categories.Add(category);
                                }
                                
                            }
                            else
                            {
                                Console.WriteLine("DB has no rows");
                            }
                        }
                    }
                }
                return Ok(categories);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryDto category)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("Database");
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    using(SqlCommand command = new SqlCommand("insertData",connection))
                    {
                        await connection.OpenAsync();
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Name",category.Name);
                        command.Parameters.AddWithValue("@Email", category.Email);
                        command.Parameters.AddWithValue("@Phone", category.Phone);
                        command.Parameters.AddWithValue("@Address", category.Address);
                        await command.ExecuteNonQueryAsync();

                    }
                    return Ok("Data Inserted Sucessfully");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error " + ex.ToString());
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPut]
        public async Task<IActionResult> Update(Category category)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("Database");
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("checkData", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", category.Id);
                        await connection.OpenAsync();
                        var result = await command.ExecuteScalarAsync();
                        if (result is int && (int)result == 1)
                        {
                            using (SqlCommand commandUpdate = new SqlCommand("updateData", connection))
                            {

                                commandUpdate.CommandType = CommandType.StoredProcedure;
                                commandUpdate.Parameters.AddWithValue("@Id", category.Id);
                                commandUpdate.Parameters.AddWithValue("@Name", category.Name);
                                commandUpdate.Parameters.AddWithValue("@Email", category.Email);
                                commandUpdate.Parameters.AddWithValue("@Phone", category.Phone);
                                commandUpdate.Parameters.AddWithValue("@Address", category.Address);
                                await commandUpdate.ExecuteNonQueryAsync();

                            }
                            return Ok("Data Sucessfully Updated");
                        }
                        else
                        {
                            return BadRequest("Data does not exists!!");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error" + ex.ToString());
                return StatusCode(500, "Internal Sever Error");
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("Database");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("checkData", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", Id);
                        await connection.OpenAsync();
                        var result = await command.ExecuteScalarAsync();
                        if (result is int && (int)result == 1)
                        {
                            using (SqlCommand commandDelete = new SqlCommand("deleteData", connection))
                            {
                                commandDelete.CommandType = CommandType.StoredProcedure;
                                commandDelete.Parameters.AddWithValue("@Id", Id);
                                await commandDelete.ExecuteNonQueryAsync();
                            }
                            return Ok("Data Deleted Sucessfully");
                        }
                        else
                        {
                            return BadRequest("Data does not exists");
                        }
                    }
                }
                
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error" + ex.ToString());
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
    }
