using System.ComponentModel.DataAnnotations;

namespace SportsLeague.API.DTOs.Request
{
    public class TournamentSponsorRequestDTO
    {
        [Required]
        public int TournamentId { get; set; }

        [Required]
        public decimal ContractAmount { get; set; }
    }
}
