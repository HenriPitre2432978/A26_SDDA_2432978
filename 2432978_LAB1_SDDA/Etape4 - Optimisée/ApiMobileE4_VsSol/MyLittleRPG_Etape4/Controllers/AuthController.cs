using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyLittleRPG_Etape4.Data.Context;
using MyLittleRPG_Etape4.Helpers;
using MyLittleRPG_Etape4.Models;
using MyLittleRPG_Etape4.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyLittleRPG_Etape4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(RPGContext context, IConfiguration config) : ControllerBase
    {
        private readonly RPGContext _context = context;
        private readonly IConfiguration _config = config;
        private readonly PasswordHasher<Utilisateur> _hasher = new();

        // POST api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest r)
        {
            string email = r.Email.Trim().ToLower();

            //find if unique existing email
            if (await _context.Utilisateur.AnyAsync(u => u.Email == email))
                return BadRequest(new { message = "Un compte existe déjà avec ce courriel." });

            //Create user no pwd
            Utilisateur user = new()
            {
                Email = email,
                Pseudo = r.Pseudo.Trim()
            };

            //set pwd to hashed request pwd input
            user.MotDePasse = _hasher.HashPassword(user, r.MotDePasse);

            //get starting valid tile
            int startX = 10;
            int startY = 10;
            Tile start = await TileHelper.GetOrCreateTile(_context, startX, startY);
            while (!start.IsTraversable && startX < WorldConfig.Width - 1)
            {
                startX++;
                start = await TileHelper.GetOrCreateTile(_context, startX, startY);
            }

            // créer perso du user
            user.Personnage = new Personnage { Nom = user.Pseudo, PositionX = startX, PositionY = startY };

            //Save all (perso+user)
            _context.Utilisateur.Add(user);
            await _context.SaveChangesAsync();

            //explorer autour du point de départ après save pour id
            await TileHelper.GetNeighboringTiles(_context, user.Personnage);

            //200 ok
            return StatusCode(StatusCodes.Status201Created, new { user.idUtilisateur, user.Pseudo });
        }

        // POST api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest r)
        {
            string email = r.Email.Trim().ToLower();

            Utilisateur? user = await _context.Utilisateur.FirstOrDefaultAsync(u => u.Email == email);
            
            PasswordVerificationResult result = new(); 
            if (user != null)
                result = _hasher.VerifyHashedPassword(user, user.MotDePasse, r.MotDePasse);
            
            // si mauvais courriel ou mauvais mdp
            if (user == null || result == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Courriel ou mot de passe invalide." });

            //200 OK et link le token à user
            return Ok(new { token = CreerToken(user), pseudo = user.Pseudo });
        }

        // créer token jwt
        private string CreerToken(Utilisateur user)
        {
            //infos publiques token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.idUtilisateur.ToString()),
                new Claim(ClaimTypes.Name, user.Pseudo)
            };

            //parse clé appsettings en clé jwt
            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            // créer jeton tel que program.cs à partir de jwt appsettings
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:ExpireMinutes")),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            // serialiser token en format comprehensible
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // POST api/Auth/logout
        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            //rien a faire car token jamais saved par le programme
            return NoContent();
        }
    }
}