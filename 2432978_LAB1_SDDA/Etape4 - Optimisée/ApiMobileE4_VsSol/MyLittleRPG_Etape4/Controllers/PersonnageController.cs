using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittleRPG_Etape4.Data.Context;
using MyLittleRPG_Etape4.Helpers;
using MyLittleRPG_Etape4.Models;
using MyLittleRPG_Etape4.Models.DTO;
using System.Security.Claims;
using static MyLittleRPG_Etape4.Models.DTO.GameDTOs;

namespace MyLittleRPG_Etape4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] //everywhere will need token login to access personnage
    public class PersonnageController(RPGContext context) : ControllerBase
    {
        private readonly RPGContext _context = context;

        private static readonly object baseError = new { message = "Personnage introuvable." };

        // GET api/Personnage/self
        [HttpGet("self")]
        public async Task<IActionResult> GetSelfPersonnage()
        {
            var perso = await GetPersonnage();
            if (perso == null)
                return NotFound(baseError);

            return Ok(await MyData(perso));
        }

        // POST api/Personnage/deplacer
        [HttpPost("deplacer")]
        public async Task<IActionResult> Deplacer(DeplacementRequest r)
        {
            var perso = await GetPersonnage();
            if (perso == null)
                return NotFound(baseError);

            //perso mort peut pas bouger
            if (perso.PointsVie <= 0)
                return BadRequest(new { message = "Votre personnage est mort. Envoyez un transfert de 200$ à 2432978@etu.cchic.ca pour le réanimer et bouger." });

            //move = ++ ou -- car DTO empêche movement < 1 de distance
            int x = perso.PositionX;
            int y = perso.PositionY;
            switch (r.Direction.ToUpper())
            {
                case "N": y--; break;
                case "S": y++; break;
                case "E": x++; break;
                case "O": x--; break;
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

            //explore auto les 8 tuiles autour
            await TileHelper.GetNeighboringTiles(_context, perso);
            await _context.SaveChangesAsync();

            //send back ok avec details (optimiser requete)
            return Ok(await MyData(perso));
        }

        // POST api/Personnage/explorer/5/5
        [HttpPost("explorer/{x}/{y}")]
        public async Task<IActionResult> Explorer(int x, int y)
        {
            var perso = await GetPersonnage();
            if (perso == null)
                return NotFound(baseError);

            if (!WorldConfig.IsValid(x, y))
                return BadRequest(new { message = "Hors du monde." });

            //seulement dans 5x5
            if (Math.Abs(x - perso.PositionX) > WorldConfig.Vision || Math.Abs(y - perso.PositionY) > WorldConfig.Vision)
                return BadRequest(new { message = "Cette tuile est trop loin." });

            //check if tile is already found to avoid a db request
            TuileExploree? foundTile = await _context.TuileExploree
                .Include(e => e.Tile)
                .FirstOrDefaultAsync(e => e.idPersonnage == perso.idPersonnage && e.X == x && e.Y == y);

            //Return DTO of tile that's found
            if (foundTile != null)
                return Ok(TuileDto.From(foundTile.Tile));

            var tile = await TileHelper.GetOrCreateTile(_context, x, y);
            _context.TuileExploree.Add(new TuileExploree { idPersonnage = perso.idPersonnage, X = x, Y = y });
            await _context.SaveChangesAsync();

            return Ok(TuileDto.From(tile));
        }

        //Find first character of current authroized user
        private async Task<Personnage?> GetPersonnage()
        {
            int userId = GetUserId();
            return await _context.Personnage.FirstOrDefaultAsync(p => p.utilisateurID == userId);
        }

        //toutes les infos de jeu du perso: stats + tuiles visibles
        private async Task<JeuDto> MyData(Personnage perso)
            => new(PersonnageDto.From(perso), await TileHelper.GetTuilesVisibles(_context, perso));

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
