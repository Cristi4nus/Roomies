using Microsoft.AspNetCore.Mvc;
using RoomiesApi.Models;

namespace RoomiesApi.Controllers
{
    [ApiController]
    [Route("api/notificari")]
    public class ControllerNotificari : ControllerBase
    {
        private readonly DatabaseService _db;

        public ControllerNotificari(DatabaseService db)
        {
            _db = db;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(int userId)
        {
            var list = await _db.GetNotificationsForUserAsync(userId);
            return Ok(list);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] Notificare notif)
        {
            await _db.AddNotificationAsync(notif.UserId, notif.Text);
            return Ok("Notificare adaugată");
        }
    }
}
