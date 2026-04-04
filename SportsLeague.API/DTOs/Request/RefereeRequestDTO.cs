using System.ComponentModel.DataAnnotations;

namespace SportsLeague.API.DTOs.Request
{
    public class RefereeRequestDTO
    {
        [Required, MaxLength(80)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(80)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(80)]
        public string Nationality { get; set; } = string.Empty;
    }
}
