using Microsoft.EntityFrameworkCore;

namespace MyLittleRPG_Etape4.Models
{
    //Tuile vue par un perso. Chaque perso a sa propre liste, donc pas un bool dans Tile
    [PrimaryKey(nameof(idPersonnage), nameof(X), nameof(Y))]
    public class TuileExploree
    {
        public int idPersonnage { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
