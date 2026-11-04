namespace MyLittleRPG_Etape4.Models
{
    public static class WorldConfig
    {
        public const int Width = 50;
        public const int Height = 50;

        public static bool IsValid(int x, int y)
            => x >= 0 && x < Width && y >= 0 && y < Height;
    }
}
