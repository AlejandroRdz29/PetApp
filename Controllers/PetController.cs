using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using PetApp.Models;

[ApiController]
[Route("api/[controller]")]
public class PetController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public PetController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private MySqlConnection GetConnection() =>
        new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));

    [HttpPost("create")]
    public IActionResult CreatePet([FromBody] Pet pet)
    {
        using var conn = GetConnection();
        using var cmd = new MySqlCommand("CreatePet", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@id", pet.IdUser);
        cmd.Parameters.AddWithValue("@pet", pet.Name);
        cmd.Parameters.AddWithValue("@sp", pet.Specie);
        cmd.Parameters.AddWithValue("@ag", pet.Age);

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();

        return Ok("Mascota creada exitosamente");
    }

    [HttpPut("update")]
    public IActionResult UpdatePet([FromBody] Pet pet)
    {
        using var conn = GetConnection();
        using var cmd = new MySqlCommand("ModifyPet", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@id", pet.IdPet);
        cmd.Parameters.AddWithValue("@name", pet.Name);
        cmd.Parameters.AddWithValue("@sp", pet.Specie);
        cmd.Parameters.AddWithValue("@ag", pet.Age);

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();

        return Ok("Mascota modificada");
    }

    [HttpDelete("{id}")]
    public IActionResult DeletePet(int id)
    {
        using var conn = GetConnection();
        using var cmd = new MySqlCommand("DeletePet", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@id", id);

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();

        return Ok("Mascota eliminada");
    }

    [HttpGet("user/{id}")]
    public IActionResult GetPetsByUser(int id)
    {
        using var conn = GetConnection();
        using var cmd = new MySqlCommand("ViewAllPet", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@id", id);
        conn.Open();

        var reader = cmd.ExecuteReader();
        var pets = new List<Pet>();

        while (reader.Read())
        {
            pets.Add(new Pet
            {
                IdPet = reader.GetInt32("idPet"),
                IdUser = reader.GetInt32("idUser"),
                Name = reader.GetString("petName"),
                Specie = reader.GetString("specie"),
                Age = reader.GetString("age")
            });
        }

        conn.Close();
        return Ok(pets);
    }

    [HttpGet("{id}")]
    public IActionResult GetPet(int id)
    {
        using var conn = GetConnection();
        using var cmd = new MySqlCommand("ViewSpecificPet", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@id", id);
        conn.Open();

        var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var pet = new Pet
            {
                IdPet = reader.GetInt32("idPet"),
                IdUser = reader.GetInt32("idUser"),
                Name = reader.GetString("petName"),
                Specie = reader.GetString("specie"),
                Age = reader.GetString("age")
            };
            conn.Close();
            return Ok(pet);
        }

        conn.Close();
        return NotFound("Mascota no encontrada");
    }
}
