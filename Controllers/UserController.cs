using Microsoft.AspNetCore.Mvc;
using System.Data;
using MySql.Data.MySqlClient;
using PetApp.Models;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public UserController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private MySqlConnection GetConnection()
    {
        return new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    }

    [HttpPost("create")]
    public IActionResult CreateUser([FromBody] User user)
    {
        try
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand("CreateUser", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@usnm", user.Username);
            cmd.Parameters.AddWithValue("@ml", user.Mail);
            cmd.Parameters.AddWithValue("@pass", user.Password);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            return Ok("Usuario creado exitosamente");
        }
        catch (Exception ex) {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] User loginUser)
    {
        try
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand("Login", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@name", loginUser.Username);
            cmd.Parameters.AddWithValue("@pass", loginUser.Password);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                var user = new User
                {
                    IdUser = reader.GetInt32("idUser"),
                    Username = reader.GetString("username"),
                    Mail = reader.GetString("mail"),
                    Password = reader.GetString("password")
                };
                return Ok(user);
            }

            return Unauthorized("Credenciales incorrectas");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        using var conn = GetConnection();
        using var cmd = new MySqlCommand("DeleteUser", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@id", id);

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();

        return Ok("Usuario eliminado");
    }
}