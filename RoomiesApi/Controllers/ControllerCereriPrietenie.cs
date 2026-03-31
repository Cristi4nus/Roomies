using Microsoft.AspNetCore.Mvc;
using RoomiesApi.Models;

namespace RoomiesApi.Controllers
{
    [ApiController]
    [Route("api/cereri")]
    public class ControllerCereriPrietenie : ControllerBase
    {
        private readonly DatabaseService _db;

        public ControllerCereriPrietenie(DatabaseService db)
        {
            _db = db;
        }

        [HttpGet("pending/{userId}")]
        public async Task<IActionResult> GetPending(int userId)
        {
            var cereri = await _db.GetPendingRequestsForUserAsync(userId);
            return Ok(cereri);
        }

        [HttpGet("haspending")]
        public async Task<IActionResult> HasPending(int senderId, int receiverId)
        {
            bool result = await _db.HasPendingRequestAsync(senderId, receiverId);
            return Ok(result);
        }

        [HttpGet("hasreverse")]
        public async Task<IActionResult> HasReverse(int senderId, int receiverId)
        {
            bool result = await _db.HasReversePendingRequestAsync(senderId, receiverId);
            return Ok(result);
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] CererePrietenie cerere)
        {
            await _db.SendFriendRequestAsync(cerere);
            return Ok("Cerere trimisă");
        }

        public record UpdateStatusRequest(int RequestId, string Status);

        [HttpPost("update")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest req)
        {
            await _db.UpdateFriendRequestStatusAsync(req.RequestId, req.Status);
            return Ok("Status actualizat");
        }
    }
}
