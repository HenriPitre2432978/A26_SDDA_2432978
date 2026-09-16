using Microsoft.EntityFrameworkCore;
using MyLittleRPG_Etape3.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyLittleRPG_Etape3.Models
{
    [PrimaryKey(nameof(X), nameof(Y))]
    public class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TileType Type { get; set; }
        public bool CanPass { get; set; }
        public string Description { get; set; }

        public Tile(int x, int y, TileType type, bool canPass, string description)
        {
            X = x;
            Y = y;
            Type = type;
            CanPass = canPass;
            Description = description;
        }
    }
}