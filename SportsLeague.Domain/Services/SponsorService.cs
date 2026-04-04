using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System.Text.RegularExpressions;

namespace SportsLeague.Domain.Services
{
    public class SponsorService : ISponsorService
    {
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
        private readonly ITournamentRepository _tournamentRepository;

        public SponsorService(
            ISponsorRepository sponsorRepository,
            ITournamentSponsorRepository tournamentSponsorRepository,
            ITournamentRepository tournamentRepository)
        {
            _sponsorRepository = sponsorRepository;
            _tournamentSponsorRepository = tournamentSponsorRepository;
            _tournamentRepository = tournamentRepository;
        }

        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            return await _sponsorRepository.GetAllAsync();
        }

        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            return await _sponsorRepository.GetByIdAsync(id);
        }

        public async Task<Sponsor> CreateAsync(Sponsor sponsor)
        {
            ValidateEmail(sponsor.ContactEmail);

            if (await _sponsorRepository.ExistsByNameAsync(sponsor.Name))
            {
                throw new InvalidOperationException($"No se puede crear un Sponsor con Name duplicado.");
            }

            return await _sponsorRepository.AddAsync(sponsor);
        }

        public async Task UpdateAsync(int id, Sponsor sponsor)
        {
            ValidateEmail(sponsor.ContactEmail);

            if (await _sponsorRepository.ExistsByNameAsync(sponsor.Name, id))
            {
                throw new InvalidOperationException($"No se puede crear un Sponsor con Name duplicado.");
            }

            var existingSponsor = await _sponsorRepository.GetByIdAsync(id);
            if (existingSponsor == null)
            {
                throw new KeyNotFoundException($"El sponsor con ID {id} no existe.");
            }

            existingSponsor.Name = sponsor.Name;
            existingSponsor.ContactEmail = sponsor.ContactEmail;
            existingSponsor.Phone = sponsor.Phone;
            existingSponsor.WebsiteUrl = sponsor.WebsiteUrl;
            existingSponsor.Category = sponsor.Category;

            await _sponsorRepository.UpdateAsync(existingSponsor);
        }

        public async Task DeleteAsync(int id)
        {
            var sponsor = await _sponsorRepository.GetByIdAsync(id);
            if (sponsor != null)
            {
                await _sponsorRepository.DeleteAsync(sponsor);
            }
        }

        public async Task LinkToTournamentAsync(int sponsorId, int tournamentId, decimal contractAmount)
        {
            // TODO: A futuro, implementar la validación del monto mínimo consultando una API externa de conversión de USD a COP.
            if (contractAmount <= 10000000m)
            {
                throw new InvalidOperationException("ContractAmount debe ser mayor a 10000000.");
            }

            var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);
            if (sponsor == null)
            {
                throw new KeyNotFoundException("No se puede vincular un Sponsor que no existe a un Tournament.");
            }

            var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
            if (tournament == null)
            {
                throw new KeyNotFoundException("No se puede vincular un Sponsor a un Tournament que no existe.");
            }

            var existingLink = await _tournamentSponsorRepository.GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
            if (existingLink != null)
            {
                throw new InvalidOperationException("No se puede duplicar la vinculación. El Sponsor ya está vinculado a este Tournament.");
            }

            var tournamentSponsor = new TournamentSponsor
            {
                TournamentId = tournamentId,
                SponsorId = sponsorId,
                ContractAmount = contractAmount,
                JoinedAt = DateTime.UtcNow
            };

            await _tournamentSponsorRepository.AddAsync(tournamentSponsor);
        }

        public async Task UnlinkFromTournamentAsync(int sponsorId, int tournamentId)
        {
            var existingLink = await _tournamentSponsorRepository.GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
            if (existingLink == null)
            {
                throw new KeyNotFoundException("La vinculación no existe.");
            }

            await _tournamentSponsorRepository.DeleteAsync(existingLink);
        }

        public async Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorAsync(int sponsorId)
        {
            var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);
            if (sponsor == null)
            {
                throw new KeyNotFoundException($"El sponsor con ID {sponsorId} no existe.");
            }

            return await _tournamentSponsorRepository.GetBySponsorIdAsync(sponsorId);
        }

        private void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new InvalidOperationException("ContactEmail debe ser un formato válido.");
            }
        }
    }
}
