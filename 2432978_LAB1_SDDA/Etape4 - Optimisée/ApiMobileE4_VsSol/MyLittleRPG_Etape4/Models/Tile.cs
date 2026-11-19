using Microsoft.EntityFrameworkCore;
using MyLittleRPG_Etape4.Models.Enums;

namespace MyLittleRPG_Etape4.Models
{
    [PrimaryKey(nameof(X), nameof(Y))]
    public class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TileType Type { get; set; }
        public bool IsTraversable { get; set; }
        public string ImgUrl { get; set; }

        public Tile(int x, int y, TileType type, bool isTraversable, string imgUrl)
        {
            X = x;
            Y = y;
            Type = type;
            IsTraversable = isTraversable;
            ImgUrl = imgUrl;
        }
    }
}