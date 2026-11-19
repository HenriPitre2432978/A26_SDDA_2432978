namespace MyLittleRPG_Etape4.Models.DTO
{
    /// <summary>
    /// Regroupement des DTO sécuritaires pour communiquer avec le WEB
    /// </summary>
    public class GameDTOs
    {
        //Record = équivalent d'un obj Classe mais en read-only et immuable

        /// <summary>
        /// Objet Tuile converti en DTO pour sécuriser la communication WEB 
        /// </summary>
        public record TuileDto(int X, int Y, string TypeTxt, bool IsTraversable, string Description, string ImgUrl)
        {
            /// <summary>
            /// Get TuileDTO object from an existing tile in parameter
            /// Calculates TypeTxt and Description by inferring on the tile obj.
            /// </summary>
            /// <param name="t">Existiing tile to get obj from</param>
            /// <returns>The TuileDTO obj</returns>
            public static TuileDto From(Tile t)
                => new(t.X, t.Y, t.Type.ToString(), t.IsTraversable, $"Tuile {t.Type} en position ({t.X},{t.Y}).", t.ImgUrl);
        }

        public record PersonnageDto(string Nom, int Niveau, int Experience, int PointsVie, int PointsVieMax, int Force, int Defense, int PositionX, int PositionY)
        {
            /// <summary>
            /// Get personnageDTO object from an existing personnage in parameter
            /// </summary>
            /// <param name="p">Existiing personnage to get obj from</param>
            /// <returns>The PersonnageDTO obj</returns>
            public static PersonnageDto From(Personnage p)
                => new(p.Nom, p.Niveau, p.Experience, p.PointsVie, p.PointsVieMax, p.Force, p.Defense, p.PositionX, p.PositionY);
        }

        /// <summary>
        /// Represents a response from "Self" and "Deplacer" call 
        /// </summary>
        /// <param name="Personnage">The updated personnage considering the self/deplacer consequences</param>
        /// <param name="Tuiles">The updated tiles list that changed from self/deplacer action</param>
        public record JeuDto(PersonnageDto Personnage, List<TuileDto> Tuiles);
    }
}
