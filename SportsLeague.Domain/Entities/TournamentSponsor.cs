namespace SportsLeague.Domain.Entities
{
    public class TournamentSponsor : AuditBase
    {
        public decimal ContractAmount { get; set; }
        public DateTime JoinedAt { get; set; }

        // Navigation Properties
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; } = null!;

        public int SponsorId { get; set; }
        public Sponsor Sponsor { get; set; } = null!;
    }
}
