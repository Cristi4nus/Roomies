using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RoomiesApi.Models;

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

            var token = GenerateJwt(user);

            return Ok(new { token, user });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CerereRegister req)
        {
            PasswordHelper.CreatePasswordHash(req.Parola, out var hash, out var salt);

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
                ParolaSalt = salt
            };

            await _db.AddMembruAsync(membru);

            return Ok("Utilizator creat");
        }

        private string GenerateJwt(Membru user)
        {
            var claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("cheia_mea_super_mega_sigura_pentru_api"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Parola { get; set; }
    }
}