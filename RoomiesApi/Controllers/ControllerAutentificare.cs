using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RoomiesApi.Models;
using RoomiesApi.Services;

namespace RoomiesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ControllerAutentificare : ControllerBase
    {
        private readonly DatabaseService _db;

        public ControllerAutentificare(DatabaseService db)
        {
            _db = db;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _db.GetMembruByEmailAsync(req.Email);
            if (user == null)
                return Unauthorized("Email sau parolă greșită");

            var ok = PasswordHelper.VerifyPassword(req.Parola, user.ParolaHash, user.ParolaSalt);
            if (!ok)
                return Unauthorized("Email sau parolă greșită");

            // dacă emailul nu e confirmat, blochează loginul
            if (user.EmailConfirmat == 0)
                return Unauthorized("EMAIL_NECONFIRMAT");

            var token = GenerateJwt(user);
            return Ok(new { token, user });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] CerereRegister req,
            [FromServices] ServiciuEmail emailService)
        {
            PasswordHelper.CreatePasswordHash(req.Parola, out var hash, out var salt);

            // generează un token unic pentru linkul de confirmare
            var tokenConfirmare = Guid.NewGuid().ToString();

            var membru = new Membru
            {
                Avatar = req.Avatar,
                Nume = req.Nume,
                Prenume = req.Prenume,
                Varsta = req.Varsta,
                Gen = req.Gen,
                Facultate = req.Facultate,
                NumarTelefon = req.NumarTelefon,
                ZonaPreferata = req.ZonaPreferata,
                BugetMaxim = req.BugetMaxim,
                PerioadaDeSedere = req.PerioadaDeSedere,
                StilDeViata = req.StilDeViata,
                PreferinteDeTrai = req.PreferinteDeTrai,
                Descriere = req.Descriere,
                Email = req.Email,
                ParolaHash = hash,
                ParolaSalt = salt,
                EmailConfirmat = 0,
                TokenConfirmare = tokenConfirmare
            };

            await _db.AddMembruAsync(membru);

            // trimite emailul de confirmare către adresa introdusă la register
            await emailService.TrimiteEmailConfirmareAsync(req.Email, tokenConfirmare);

            // verifică alarme compatibile
            var alarme = await _db.GetAllAlarmeAsync();
            foreach (var alarma in alarme)
            {
                if (alarma.UserId == membru.Id) continue;
                bool compatibil = true;
                if (!string.IsNullOrWhiteSpace(alarma.Gen) &&
                    !string.Equals(alarma.Gen, membru.Gen, StringComparison.OrdinalIgnoreCase))
                    compatibil = false;
                if (!string.IsNullOrWhiteSpace(alarma.Zona) &&
                    !(membru.ZonaPreferata?.Split(',').Select(z => z.Trim().ToLower())
                        .Contains(alarma.Zona.ToLower()) ?? false))
                    compatibil = false;
                if (!string.IsNullOrWhiteSpace(alarma.StilViata) &&
                    !string.Equals(alarma.StilViata, membru.StilDeViata, StringComparison.OrdinalIgnoreCase))
                    compatibil = false;
                if (!string.IsNullOrWhiteSpace(alarma.Preferinte) &&
                    !(membru.PreferinteDeTrai?.Split(',').Select(p => p.Trim().ToLower())
                        .Contains(alarma.Preferinte.ToLower()) ?? false))
                    compatibil = false;
                if (compatibil)
                    await _db.AddNotificationAsync(alarma.UserId,
                        $"Un utilizator compatibil cu filtrele tale a apărut: {membru.Nume} {membru.Prenume}!");
            }

            return Ok("Utilizator creat. Verifică emailul pentru confirmare.");
        }

        private string GenerateJwt(Membru user)
        {
            var claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("cheia_mea_super_mega_sigura_pentru_api"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Parola { get; set; }
    }
}