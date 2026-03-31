using Microsoft.AspNetCore.Mvc;
using RoomiesApi.Models;

namespace RoomiesApi.Controllers
{
    [ApiController]
    [Route("api/mesaje")]
    public class ControllerMesaje : ControllerBase
    {
        private readonly DatabaseService _db;

        public ControllerMesaje(DatabaseService db)
        {
            _db = db;
        }

        [HttpGet("{user1}/{user2}")]
        public async Task<IActionResult> GetConversation(int user1, int user2)
        {
            var mesaje = await _db.GetConversationAsync(user1, user2);
            return Ok(mesaje);
        }
    }
}