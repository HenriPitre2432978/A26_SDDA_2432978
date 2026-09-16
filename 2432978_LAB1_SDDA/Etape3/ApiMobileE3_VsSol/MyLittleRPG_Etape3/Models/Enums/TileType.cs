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

    }
}
