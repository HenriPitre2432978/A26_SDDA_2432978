using System.ComponentModel.DataAnnotations;

namespace MyLittleRPG_Etape4.Models.DTO
{
    public class DeplacementRequest
    {
        //N, S, E ou O
        //Regex: oblige nseo || NSEO
        [Required, RegularExpression("^[NSEOnseo]$")]
        public string Direction { get; set; } = string.Empty;
    }
}
