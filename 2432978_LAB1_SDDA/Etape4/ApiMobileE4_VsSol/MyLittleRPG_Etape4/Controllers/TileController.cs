using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittleRPG_Etape4.Data.Context;
using MyLittleRPG_Etape4.Models;
using MyLittleRPG_Etape4.Models.Enums;
using Weighted_Randomizer;

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

            var tile = await _context.Tile.FindAsync(x, y);

            if (tile == null)
            {
                //Créer nouvelle tuile
                //https://github.com/BlueRaja/Weighted-Item-Randomizer-for-C-Sharp/wiki/Getting-Started

                DynamicWeightedRandomizer<string> r = [];
                foreach (TileType type in Enum.GetValues<TileType>())
                    r.Add(Enum.GetName(type) ?? "N/A", type.Weight());

                string chosenTypeTxt = r.NextWithReplacement();
                TileType current = (TileType)Enum.Parse(typeof(TileType), chosenTypeTxt);

                tile = new(x, y, current, current.IsTraversable(), $"Tuile {Enum.GetName(current)} en position ({x},{y}).", current.ImgUrl());

                //Save new tile to db
                _context.Tile.Add(tile);
                await _context.SaveChangesAsync();
            }

            return tile;
        }

    }

}

