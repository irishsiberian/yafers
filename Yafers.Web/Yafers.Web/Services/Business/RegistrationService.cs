using Yafers.Web.Services.Business.Interfaces;
using Yafers.Web.Data;
using Yafers.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Yafers.Web.Services.Business
{
        public class RegistrationService : IRegistrationService
        {
            private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

            public RegistrationService(IDbContextFactory<ApplicationDbContext> dbFactory)
            {
                _dbFactory = dbFactory;
            }

            public async Task<int> CreateDancerRegistrationAsync(int dancerId, int feisId, string createdBy, CancellationToken ct = default)
            {
                await using var db = _dbFactory.CreateDbContext();
                var dr = new DancerRegistration
                {
                    DancerId = dancerId,
                    FeisId = feisId,
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedBy = createdBy ?? "",
                    InCartForUser = createdBy,
                    IsInCart = true
                };
                db.DancerRegistrations.Add(dr);
                await db.SaveChangesAsync(ct);
                return dr.Id;
            }

            public async Task DeleteDancerRegistrationAsync(int dancerRegistrationId, CancellationToken ct = default)
            {
                await using var db = _dbFactory.CreateDbContext();
                var dr = await db.DancerRegistrations.FirstOrDefaultAsync(d => d.Id == dancerRegistrationId, ct);
                if (dr != null)
                {
                    // delete related competition registrations
                    var crs = await db.CompetitionRegistrations.Where(cr => cr.DancerRegistrationId == dancerRegistrationId).ToListAsync(ct);
                    if (crs.Any())
                    {
                        foreach (var cr in crs)
                        {
                            cr.IsDeleted = true;
                            cr.DeletedAtUtc = DateTime.UtcNow;
                            db.CompetitionRegistrations.Update(cr);
                        }
                    }

                    dr.IsDeleted = true;
                    dr.DeletedAtUtc = DateTime.UtcNow;
                    db.DancerRegistrations.Update(dr);
                    await db.SaveChangesAsync(ct);
                }
            }

            public async Task<bool> AddCompetitionRegistrationAsync(int dancerRegistrationId, int dancerId, int competitionId, int feisId, string registrarId, CancellationToken ct = default)
            {
                await using var db = _dbFactory.CreateDbContext();

                var exists = await db.CompetitionRegistrations.AnyAsync(cr =>
                    cr.DancerRegistrationId == dancerRegistrationId && cr.CompetitionId == competitionId, ct);
                if (exists) return false;

                var crNew = new CompetitionRegistration
                {
                    DancerId = dancerId,
                    DancerRegistrationId = dancerRegistrationId,
                    CompetitionId = competitionId,
                    FeisId = feisId,
                    RegistrarId = registrarId,
                    IsInCart = true,
                    InCartForUser = registrarId,
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedBy = registrarId ?? ""
                };
                db.CompetitionRegistrations.Add(crNew);
                await db.SaveChangesAsync(ct);
                return true;
            }

            public async Task RemoveCompetitionRegistrationAsync(int dancerRegistrationId, int competitionId, CancellationToken ct = default)
            {
                await using var db = _dbFactory.CreateDbContext();
                var items = await db.CompetitionRegistrations
                    .Where(cr => cr.DancerRegistrationId == dancerRegistrationId && cr.CompetitionId == competitionId && cr.IsInCart)
                    .ToListAsync(ct);
                if (!items.Any()) return;
                foreach (var item in items)
                {
                    item.IsDeleted = true;
                    item.DeletedAtUtc = DateTime.UtcNow;
                    db.CompetitionRegistrations.Update(item);
                }
                await db.SaveChangesAsync(ct);
            }

            public async Task<HashSet<int>> GetCompetitionRegistrationIdsAsync(int dancerRegistrationId, CancellationToken ct = default)
            {
                await using var db = _dbFactory.CreateDbContext();
                var set = await db.CompetitionRegistrations
                    .AsNoTracking()
                    .Where(cr => cr.DancerRegistrationId == dancerRegistrationId && cr.IsInCart)
                    .Select(cr => cr.CompetitionId)
                    .ToListAsync(ct);
                return set.ToHashSet();
            }

            public async Task<List<DancerRegistration>> GetDancerRegistrationsInCartAsync(int feisId, string inCartForUser, CancellationToken ct = default)
            {
                await using var db = _dbFactory.CreateDbContext();
                return await db.DancerRegistrations
                    .AsNoTracking()
                    .Where(dr => dr.FeisId == feisId && dr.IsInCart && dr.InCartForUser == inCartForUser && !dr.IsDeleted)
                    .Include(dr => dr.CompetitionRegistrations.Where(cr => cr.IsInCart && cr.InCartForUser == inCartForUser))
                    .ToListAsync(ct);
            }
    }
}
