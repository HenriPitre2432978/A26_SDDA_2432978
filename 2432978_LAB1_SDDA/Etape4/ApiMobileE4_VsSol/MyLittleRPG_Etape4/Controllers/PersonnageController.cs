using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MyLittleRPG_Etape4.Data.Context;
using MyLittleRPG_Etape4.Helpers;
using MyLittleRPG_Etape4.Models;
using MyLittleRPG_Etape4.Models.DTO;
using System.Security.Claims;

namespace MyLittleRPG_Etape4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] //everywhere will need token login to access personnage
    public class PersonnageController(RPGContext context) : ControllerBase
    {
        private readonly RPGContext _context = context;

        // GET api/Personnage/self
        [HttpGet("self")]
        public async Task<IActionResult> GetSelfPersonnage()
        {
            var perso = await GetPersonnage();
            if (perso == null)
                return NotFound();

            return Ok(await MyData(perso));
        }

        // POST api/Personnage/deplacer
        [HttpPost("deplacer")]
        public async Task<IActionResult> Deplacer(DeplacementRequest r)
        {
            var perso = await GetPersonnage();
            if (perso == null)
                return NotFound();

            //perso mort peut pas bouger
            if (perso.PointsVie <= 0)
                return BadRequest(new { message = "Votre personnage est mort. Envoyez un transfert de 200$ à 2432978@etu.cchic.ca pour le réanimer et bouger." });

            //case visée dist 1 
            int x = perso.PositionX;
            int y = perso.PositionY;
            switch (r.Direction.ToUpper())
            {
                case "N": y--; break;
                case "S": y++; break;
                case "E": x++; break;
                case "O": x--; break;
                default:
                    return BadRequest(new { message = "Direction invalide (N, S, E ou O)." });
            }

            if (!WorldConfig.IsValid(x, y))
                return BadRequest(new { message = "Vous êtes au bord du monde." });

            var tile = await _context.Tile.FindAsync(x, y);
            if (tile == null)
                return BadRequest(new { message = "Cette tuile n'existe pas." });

            if (!tile.IsTraversable)
                return BadRequest(new { message = $"Impossible de traverser: {tile.TypeTxt}." });

            perso.PositionX = x;
            perso.PositionY = y;
            await _context.SaveChangesAsync();

            //explore auto les 8 tuiles autour
            await TileHelper.GetNeighboringTiles(_context, perso);

            //send back ok avec details (optimiser requete)
            return Ok(await MyData(perso));
        }

        // POST api/Personnage/explorer/5/5
        [HttpPost("explorer/{x}/{y}")]
        public async Task<IActionResult> Explorer(int x, int y)
        {
            var perso = await GetPersonnage();
            if (perso == null)
                return NotFound();

            if (!WorldConfig.IsValid(x, y))
                return BadRequest(new { message = "Hors du monde." });

            //seulement dans 5x5
            if (Math.Abs(x - perso.PositionX) > WorldConfig.Vision || Math.Abs(y - perso.PositionY) > WorldConfig.Vision)
                return BadRequest(new { message = "Cette tuile est trop loin." });

            var tile = await TileHelper.GetOrCreateTile(_context, x, y);
            await TileHelper.MarquerExploree(_context, perso.idPersonnage, x, y);
            await _context.SaveChangesAsync();

            return Ok(tile);
        }

        //Find first character of current authroized user
        private async Task<Personnage?> GetPersonnage()
        {
            int userId = GetUserId();
            return await _context.Personnage.FirstOrDefaultAsync(p => p.utilisateurID == userId);
        }

        /// <summary>
        /// Get all relevant game data for specified perosnnage
        /// </summary>
        /// <param name="perso">le personnage en ligne</param>
        /// <returns>new obj avec infos personnage et tuiles chargées</returns>
        private async Task<object> MyData(Personnage perso)
        {
            return new
            {
                personnage = new
                {
                    perso.idPersonnage,
                    perso.Nom,
                    perso.Niveau,
                    perso.Experience,
                    perso.PointsVie,
                    perso.PointsVieMax,
                    perso.Force,
                    perso.Defense,
                    perso.PositionX,
                    perso.PositionY
                },
                tuiles = await TileHelper.GetTuilesVisibles(_context, perso)
            };
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    }
}
