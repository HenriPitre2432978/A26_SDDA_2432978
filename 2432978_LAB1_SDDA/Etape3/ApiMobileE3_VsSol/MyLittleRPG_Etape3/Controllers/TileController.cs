using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using MyLittleRPG_Etape3.Data.Context;
using MyLittleRPG_Etape3.Models;
using MyLittleRPG_Etape3.Models.Enums;

namespace MyLittleRPG_Etape3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TileController : ControllerBase
    {
        private readonly RPGContext _context;
        public TileController(RPGContext context) => _context = context;


        //GET api/Tiles/5
        [HttpGet("{x}/{y}")]
        public async Task<ActionResult<Tile>> GetTile(int x, int y)
        {
            var tile = await _context.Tile.FindAsync(x, y);

            if (tile == null)
            {
                //Créer nouvelle tuile
                Random r = new Random();
                TileType type = (TileType)r.Next(0, Enum.GetValues(typeof(TileType)).Length);

                tile = new Tile(x, y, type, type.IsPassable(), $"Tuile {Enum.GetName(type)} en position ({x},{y})."); ;
            }


            return tile;
        }

    }

}

