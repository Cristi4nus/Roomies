using Microsoft.AspNetCore.Mvc;
using RoomiesApi.Models;

namespace RoomiesApi.Controllers
{
    [ApiController]
    [Route("api/prieteni")]
    public class PrieteniController : ControllerBase
    {
        private readonly DatabaseService _db;

        public PrieteniController(DatabaseService db)
        {
            _db = db;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetPrieteni(int userId)
        {
            var ids = await _db.GetPrieteniIdsAsync(userId);

            var prieteni = new List<Membru>();

            foreach (var id in ids)
            {
                var user = await _db.GetMembruByIdAsync(id);
                if (user != null)
                    prieteni.Add(user);
            }

            return Ok(prieteni);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddPrieten(int user1, int user2)
        {
            await _db.AddPrietenAsync(user1, user2);
            return Ok("Prieten adăugat");
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemovePrieten(int user1, int user2)
        {
            await _db.RemovePrietenAsync(user1, user2);
            return Ok("Prieten șters");
        }
        [HttpGet("arefriends")]
        public async Task<IActionResult> AreFriends(int user1, int user2)
        {
            bool result = await _db.AreFriendsAsync(user1, user2);
            return Ok(result);
        }

    }

}
