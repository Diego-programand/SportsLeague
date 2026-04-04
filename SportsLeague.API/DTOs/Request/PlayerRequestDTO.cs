using System.ComponentModel.DataAnnotations;
using SportsLeague.Domain.Enums;

namespace SportsLeague.API.DTOs.Request
{
    public class PlayerRequestDTO
    {
        [Required, MaxLength(80)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(80)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public int Number { get; set; }

        [Required]
        public PlayerPosition Position { get; set; }

        [Required]
        public int TeamId { get; set; }
    }
}
