using Microsoft.EntityFrameworkCore;
using Yafers.Web.Data;
using Yafers.Web.Data.Entities;
using Yafers.Web.Services.Feiseanna;

namespace Yafers.Web.Services.Feiseanna
{
    public class FeisService : IFeisService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public FeisService(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<Data.Entities.Feis>> GetAvailableFeiseannaAsync(CancellationToken ct = default)
        {
            await using var db = _dbFactory.CreateDbContext();
            return await db.Feiseanna
                .AsNoTracking()
                .Where(f => !f.IsDeleted && f.RegistrationOpenDate <= DateTime.UtcNow && f.RegistrationCloseDate >= DateTime.UtcNow)
                .OrderBy(f => f.FeisDate)
                .ToListAsync(ct);
        }

        public async Task<bool> UserHasCartItemsAsync(string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(userId)) return false;
            await using var db = _dbFactory.CreateDbContext();
            return await db.CompetitionRegistrations.AnyAsync(cr => cr.IsInCart && cr.RegistrarId == userId, ct);
        }

        public async Task<Feis?> GetFeisWithSyllabusAsync(int feisId, CancellationToken ct = default)
        {
            await using var db = _dbFactory.CreateDbContext();
            return await db.Feiseanna
                .AsNoTracking()
                .Include(f => f.Syllabus)
                .FirstOrDefaultAsync(f => f.Id == feisId && !f.IsDeleted, ct);
        }

        public async Task<List<Competition>> GetCompetitionsForSyllabusAsync(int syllabusId, CancellationToken ct = default)
        {
            await using var db = _dbFactory.CreateDbContext();
            return await db.SyllabusCompetitions
                .AsNoTracking()
                .Include(x=> x.Competition)
                .Where(c => c.SyllabusId == syllabusId)
                .OrderBy(c => c.RegistrationOrder)
                .Select(x => x.Competition)
                .ToListAsync(ct);
        }
    }
}