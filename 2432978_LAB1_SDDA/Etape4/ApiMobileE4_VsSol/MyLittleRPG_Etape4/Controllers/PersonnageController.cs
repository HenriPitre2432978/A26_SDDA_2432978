using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MyLittleRPG_Etape4.Data.Context;
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
            int userId = GetUserId();

            //Find first character of current authroized user
            var perso = await _context.Personnage.FirstOrDefaultAsync(p => p.utilisateurID == userId);
            if (perso == null)
                return NotFound();

            return Ok(new
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
            });
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    }
}
