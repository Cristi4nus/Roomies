using Microsoft.AspNetCore.Mvc;
using RoomiesApi.Models;

namespace RoomiesApi.Controllers
{
    [ApiController]
    [Route("api/alarme")]
    public class ControllerAlarme : ControllerBase
    {
        private readonly DatabaseService _db;

        public ControllerAlarme(DatabaseService db)
        {
            _db = db;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddAlarma([FromBody] AlarmaFiltre alarma)
        {
            await _db.AddAlarmaAsync(alarma);
            return Ok("Alarma salvată");
        }
    }
}