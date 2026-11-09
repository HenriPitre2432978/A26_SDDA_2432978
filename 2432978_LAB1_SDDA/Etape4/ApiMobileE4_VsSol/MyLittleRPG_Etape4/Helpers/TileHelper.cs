using Microsoft.EntityFrameworkCore;
using MyLittleRPG_Etape4.Data.Context;
using MyLittleRPG_Etape4.Models;
using MyLittleRPG_Etape4.Models.Enums;
using Weighted_Randomizer;

namespace MyLittleRPG_Etape4.Helpers
{
    //Code de tuiles partagé entre TileController, AuthController et PersonnageController
    public static class TileHelper
    {
        //Retourne la tuile, ou la crée si elle existe pas encore
        public static async Task<Tile> GetOrCreateTile(RPGContext context, int x, int y)
        {
            var tile = await context.Tile.FindAsync(x, y);

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
                context.Tile.Add(tile);
                await context.SaveChangesAsync();
            }

            return tile;
        }

        //Ajoute la tuile dans les tuiles explorées du perso (si pas déjà là)
        public static async Task MarquerExploree(RPGContext context, int idPersonnage, int x, int y)
        {
            bool dejaVue = await context.TuileExploree.AnyAsync(e => e.idPersonnage == idPersonnage && e.X == x && e.Y == y);
            if (!dejaVue)
                context.TuileExploree.Add(new TuileExploree { idPersonnage = idPersonnage, X = x, Y = y });
        }

        //Explore les 8 tuiles autour du perso + celle où il est
        public static async Task GetNeighboringTiles(RPGContext context, Personnage perso)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int x = perso.PositionX + dx;
                    int y = perso.PositionY + dy;

                    if (!WorldConfig.IsValid(x, y))
                        continue;

                    await GetOrCreateTile(context, x, y);
                    await MarquerExploree(context, perso.idPersonnage, x, y);
                }
            }

            await context.SaveChangesAsync();
        }

        //Toutes les tuiles explorées par le perso dans son rayon de vision (5x5)
        public static async Task<List<Tile>> GetTuilesVisibles(RPGContext context, Personnage perso)
        {
            int minX = perso.PositionX - WorldConfig.Vision;
            int maxX = perso.PositionX + WorldConfig.Vision;
            int minY = perso.PositionY - WorldConfig.Vision;
            int maxY = perso.PositionY + WorldConfig.Vision;

            var explorees = await context.TuileExploree
                .Where(e => e.idPersonnage == perso.idPersonnage && e.X >= minX && e.X <= maxX && e.Y >= minY && e.Y <= maxY)
                .ToListAsync();

            List<Tile> tuiles = [];
            foreach (var e in explorees)
            {
                var tile = await context.Tile.FindAsync(e.X, e.Y);
                if (tile != null)
                    tuiles.Add(tile);
            }

            return tuiles;
        }
    }
}
