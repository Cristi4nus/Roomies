using Microsoft.AspNetCore.Mvc;
using RoomiesApi.Models;

namespace RoomiesApi.Controllers
{
    [ApiController]
    [Route("api/debug")]
    public class DebugController : ControllerBase
    {
        private readonly DatabaseService _db;

        public DebugController(DatabaseService db)
        {
            _db = db;
        }

        /// <summary>
        /// ENDPOINT PENTRU DEBUG - Verifica ce e salvat în baza de date
        /// Accesează: GET /api/debug/user/{email}
        /// 
        /// Exemplu: http://localhost:5137/api/debug/user/utilizatoriulia19@gmail.com
        /// </summary>
        [HttpGet("user/{email}")]
        public async Task<IActionResult> GetUserDebugInfo(string email)
        {
            try
            {
                var user = await _db.GetMembruByEmailAsync(email);

                if (user == null)
                    return NotFound(new { mesaj = "Utilizator nu găsit", email = email });

                // Returnează toate datele utilizatorului, inclusiv zonele și preferințele
                return Ok(new
                {
                    id = user.Id,
                    email = user.Email,
                    nume = user.Nume,
                    prenume = user.Prenume,

                    // ← IMPORTANT: Asta vrem să verificăm
                    zonaPreferata = user.ZonaPreferata,
                    zonaPreferata_lungime = user.ZonaPreferata?.Length ?? 0,
                    zonaPreferata_splitata = user.ZonaPreferata?.Split(',').Select(z => z.Trim()).ToList() ?? new List<string>(),

                    preferinteDeTrai = user.PreferinteDeTrai,
                    preferinteDeTrai_lungime = user.PreferinteDeTrai?.Length ?? 0,
                    preferinteDeTrai_splitata = user.PreferinteDeTrai?.Split(',').Select(p => p.Trim()).ToList() ?? new List<string>(),

                    emailConfirmat = user.EmailConfirmat,
                    varsta = user.Varsta,
                    gen = user.Gen,
                    stilDeViata = user.StilDeViata
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { eroare = ex.Message });
            }
        }

        /// <summary>
        /// Arată TOȚI utilizatorii cu zonele și preferințele lor
        /// Accesează: GET /api/debug/all-users
        /// </summary>
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsersDebug()
        {
            try
            {
                var users = await _db.GetAllMembriAsync();

                var result = users.Select(u => new
                {
                    id = u.Id,
                    email = u.Email,
                    nume = $"{u.Nume} {u.Prenume}",
                    zonaPreferata = u.ZonaPreferata,
                    preferinteDeTrai = u.PreferinteDeTrai,
                    emailConfirmat = u.EmailConfirmat
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { eroare = ex.Message });
            }
        }
    }
}