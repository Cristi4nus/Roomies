using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RoomiesApi.Models;

namespace RoomiesApi.Controllers
{
    [ApiController]
    [Route("api/membri")]
    public class MembriController : ControllerBase
    {
        private readonly DatabaseService _db;

        public MembriController(DatabaseService db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var membri = await _db.GetAllMembriAsync();
            return Ok(membri);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var membru = await _db.GetMembruByIdAsync(id);
            if (membru == null)
                return NotFound();

            return Ok(membru);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Membru membru)
        {
            if (id != membru.Id)
                return BadRequest("Id mismatch");

            await _db.UpdateMembruAsync(membru);
            return Ok("Membru actualizat");
        }
    }
}
