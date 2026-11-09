using System.ComponentModel.DataAnnotations;

namespace MyLittleRPG_Etape4.Models.DTO
{
    public class DeplacementRequest
    {
        //N, S, E ou O
        [Required]
        public string Direction { get; set; } = string.Empty;
    }
}
