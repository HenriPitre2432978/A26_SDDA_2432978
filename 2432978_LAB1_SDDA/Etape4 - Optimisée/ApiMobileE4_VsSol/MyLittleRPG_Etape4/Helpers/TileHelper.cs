using Microsoft.EntityFrameworkCore;
using MyLittleRPG_Etape4.Data.Context;
using MyLittleRPG_Etape4.Models;
using MyLittleRPG_Etape4.Models.DTO;
using MyLittleRPG_Etape4.Models.Enums;
using System.Reflection.Metadata.Ecma335;
using Weighted_Randomizer;
using static MyLittleRPG_Etape4.Models.DTO.GameDTOs;

namespace MyLittleRPG_Etape4.Helpers
{
    /// <summary>
    /// Service Partagé entre TileController, AuthController et PersonnageController
    /// </summary>
    public static class TileHelper
    {
        //https://github.com/BlueRaja/Weighted-Item-Randomizer-for-C-Sharp/wiki/Getting-Started
        //clé int car un enum implémente pas IComparable<T> (exigé par la librairie)
        private static readonly DynamicWeightedRandomizer<int> _randomizer = CreerRandomizer();

        private static DynamicWeightedRandomizer<int> CreerRandomizer()
        {
            DynamicWeightedRandomizer<int> r = [];
            foreach (TileType type in Enum.GetValues<TileType>())
                r.Add((int)type, type.Weight());

            return r;
        }

        /// <summary>
        /// Retourne la tuile, ou la crée si elle existe pas encore
        /// N'enregistre pas dans la BD en cas de création. L'ajoute au given context
        /// </summary>
        public static async Task<Tile> GetOrCreateTile(RPGContext context, int x, int y)
        {
            Tile? tile = await context.Tile.FindAsync(x, y);

            if (tile == null)
            {
                tile = CreerTuile(x, y);
                context.Tile.Add(tile);
            }

            return tile;
        }

        private static Tile CreerTuile(int x, int y)
        {
            TileType type;

            type = (TileType)_randomizer.NextWithReplacement();

            return new Tile(x, y, type, type.IsTraversable(), type.ImgUrl());
        }

        public static async Task<Tile> FindStartingTile(RPGContext context)
        {
            const int dX = 10;
            const int dY = 10;

            //find nearest traversableTile for safe spawn
            Tile? depart = await context.Tile
                .Where(t => t.IsTraversable).OrderBy(t => Math.Abs(t.X - dX) + Math.Abs(t.Y - dY)).FirstOrDefaultAsync();

            //if found in bd, break now and spawn there
            if (depart != null)
                return depart;

            //if no safe tile found, create a safe one in (10,10) and break with it
            for (int y = dY; y < WorldConfig.Height; y++)
            {
                for (int x = dX; x < WorldConfig.Width; x++)
                {
                    Tile tile = await GetOrCreateTile(context, x, y);
                    if (tile.IsTraversable) return tile;
                }
            }

            //si ça arrive je me roule en boule à terre (safe exception handler)
            throw new InvalidOperationException("Aucune tuile de départ traversable.");
        }


        //Explore les 8 tuiles autour du perso + celle où il est
        public static async Task GetNeighboringTiles(RPGContext context, Personnage perso)
        {
            int minX = perso.PositionX - 1;
            int maxX = perso.PositionX + 1;
            int minY = perso.PositionY - 1;
            int maxY = perso.PositionY + 1;

            await context.Tile
                .Where(t => t.X >= minX && t.X <= maxX && t.Y >= minY && t.Y <= maxY)
                .LoadAsync();

            await context.TuileExploree
                .Where(e => e.idPersonnage == perso.idPersonnage && e.X >= minX && e.X <= maxX && e.Y >= minY && e.Y <= maxY)
                .LoadAsync();

            for (int dx = minX; dx <= maxX; dx++)
            {
                for (int dy = minY; dy <= maxY; dy++)
                {
                    if (!WorldConfig.IsValid(dx, dy))
                        continue;

                    //Local = tuiles chargées + elles ajoutées pas encore saved
                    if (!context.Tile.Local.Any(t => t.X == dx && t.Y == dy))
                        context.Tile.Add(CreerTuile(dx, dy));

                    if (!context.TuileExploree.Local.Any(e => e.idPersonnage == perso.idPersonnage && e.X == dx && e.Y == dy))
                        context.TuileExploree.Add(new TuileExploree { idPersonnage = perso.idPersonnage, X = dx, Y = dy });
                }
            }
        }

        //Toutes les tuiles explorées par le perso dans son rayon de vision (5x5)
        public static async Task<List<TuileDto>> GetTuilesVisibles(RPGContext context, Personnage perso)
        {
            int minX = perso.PositionX - WorldConfig.Vision;
            int maxX = perso.PositionX + WorldConfig.Vision;
            int minY = perso.PositionY - WorldConfig.Vision;
            int maxY = perso.PositionY + WorldConfig.Vision;

            //Select(e => e.Tile) = EF fait le JOIN avec Tile dans la même requête
            List<Tile> tuiles = await context.TuileExploree
                .Where(e => e.idPersonnage == perso.idPersonnage && e.X >= minX && e.X <= maxX && e.Y >= minY && e.Y <= maxY)
                .Select(e => e.Tile)
                .AsNoTracking()
                .ToListAsync();

            return tuiles.Select(TuileDto.From).ToList();
        }

    }
}
