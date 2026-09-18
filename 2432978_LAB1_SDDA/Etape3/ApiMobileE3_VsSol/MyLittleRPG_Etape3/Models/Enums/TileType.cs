namespace MyLittleRPG_Etape3.Models.Enums
{
    public enum TileType
    {
        Herbe,
        Eau,
        Montagne,
        Forêt,
        Ville,
        Route
    }
    public static class TileTypeExtensions
    {
        // Méthode d'extension pratique ; on peut l'appeler comme tile.IsPassable()
        public static bool IsPassable(this TileType t) => t != TileType.Eau && t != TileType.Montagne;

        public static string ImgUrl(this TileType t)
        {
            switch (t)
            {
                case TileType.Herbe:
                    return "/images/Plains.png";
                case TileType.Eau:
                    return "/images/River.png";
                case TileType.Montagne:
                    return "/images/Mountain.png";
                case TileType.Forêt:
                    return "/images/Forest.png";
                case TileType.Ville:
                    return "/images/Town.png";
                case TileType.Route:
                    return "/images/Road.png";
                default:
                    return string.Empty;
            }
        }
    }
}
