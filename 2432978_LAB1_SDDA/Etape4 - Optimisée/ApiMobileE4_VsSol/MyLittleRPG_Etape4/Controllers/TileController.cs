using Microsoft.AspNetCore.Mvc;
using MyLittleRPG_Etape4.Data.Context;
using MyLittleRPG_Etape4.Helpers;
using MyLittleRPG_Etape4.Models;

namespace MyLittleRPG_Etape4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TileController : ControllerBase
    {
        private readonly RPGContext _context;
        public TileController(RPGContext context) => _context = context;

        //GET api/Tile/5/5
        [HttpGet("{x}/{y}")]
        public async Task<ActionResult<Tile>> GetTile(int x, int y)
        {
            if (!WorldConfig.IsValid(x, y))
                return NotFound();

            return await TileHelper.GetOrCreateTile(_context, x, y);
        }

    }

}

