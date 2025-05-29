using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using PetApp.Models;
using System.Data;
using PetApp.Models;

[ApiController]
[Route("api/[controller]")]
public class AlarmController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AlarmController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private MySqlConnection GetConnection() =>
        new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));

    [HttpPost("create")]
    public IActionResult CreateAlarm([FromBody] Alarm alarm)
    {
        using var conn = GetConnection();
        using var cmd = new MySqlCommand("CreateAlarm", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@id", alarm.IdUser);
        cmd.Parameters.AddWithValue("@name", alarm.Title);
        cmd.Parameters.AddWithValue("@hr", alarm.Hour);
        cmd.Parameters.AddWithValue("@freq", alarm.Frequency);

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();

        return Ok("Alarma creada exitosamente");
    }

    [HttpPut("update")]
    public IActionResult UpdateAlarm([FromBody] Alarm alarm)
    {
        using var conn = GetConnection();
        using var cmd = new MySqlCommand("ModifyAlarm", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@id", alarm.IdAlarm);
        cmd.Parameters.AddWithValue("@name", alarm.Title);
        cmd.Parameters.AddWithValue("@hr", alarm.Hour);
        cmd.Parameters.AddWithValue("@freq", alarm.Frequency);

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();

        return Ok("Alarma modificada");
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteAlarm(int id)
    {
        using var conn = GetConnection();
        using var cmd = new MySqlCommand("DeleteAlarm", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@id", id);

        conn.Open();
        cmd.ExecuteNonQuery();
        conn.Close();

        return Ok("Alarma eliminada");
    }

    [HttpGet("user/{id}")]
    public IActionResult GetAlarmsByUser(int id)
    {
        using var conn = GetConnection();
        using var cmd = new MySqlCommand("ViewAllAlarm", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@id", id);
        conn.Open();

        var reader = cmd.ExecuteReader();
        var alarms = new List<Alarm>();

        while (reader.Read())
        {
            alarms.Add(new Alarm
            {
                IdAlarm = reader.GetInt32("idAlarm"),
                IdUser = reader.GetInt32("idUser"),
                Title = reader.GetString("alarmName"),
                Hour = reader.GetString("hour"),
                Frequency = reader.GetString("frecuency")
            });
        }

        conn.Close();
        return Ok(alarms);
    }

    [HttpGet("{id}")]
    public IActionResult GetAlarm(int id)
    {
        using var conn = GetConnection();
        using var cmd = new MySqlCommand("ViewSpecificAlarm", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@id", id);
        conn.Open();

        var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var alarm = new Alarm
            {
                IdAlarm = reader.GetInt32("idAlarm"),
                IdUser = reader.GetInt32("idUser"),
                Title = reader.GetString("alarmName"),
                Hour = reader.GetString("hour"),
                Frequency = reader.GetString("frecuency")
            };
            conn.Close();
            return Ok(alarm);
        }

        conn.Close();
        return NotFound("Alarma no encontrada");
    }
}
