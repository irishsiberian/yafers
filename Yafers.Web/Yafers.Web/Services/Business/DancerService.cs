using Microsoft.EntityFrameworkCore;
using Yafers.Web.Data;
using Yafers.Web.Data.Entities;
using Yafers.Web.Services.Business.Interfaces;

namespace Yafers.Web.Services.Business
{
    public class DancerService : IDancerService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public DancerService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _dbFactory = contextFactory;
        }

        public async Task<Dancer?> GetByUserAsync(string userId)
        {
            await using var db = _dbFactory.CreateDbContext();
            return await db.Dancers.FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task<List<Dancer>> GetDancersForUserAsDancerAsync(string userId, CancellationToken ct = default)
        {
            await using var db = _dbFactory.CreateDbContext();
            var result = await db.Dancers.AsNoTracking()
                .Where(d => d.UserId == userId && !d.IsDeleted)
                .ToListAsync(ct);
            return result;
        }

        public async Task<List<Dancer>> GetDancersForParentAsync(string parentUserId, CancellationToken ct = default)
        {
            await using var db = _dbFactory.CreateDbContext();
            return await db.Dancers.AsNoTracking()
                .Where(d => d.DancerParentUserId == parentUserId && !d.IsDeleted)
                .ToListAsync(ct);
        }

        public async Task<List<Dancer>> GetDancersForTeacherSchoolAsync(int? schoolId, CancellationToken ct = default)
        {
            if (schoolId == null) return new();
            await using var db = _dbFactory.CreateDbContext();
            return await db.Dancers.AsNoTracking()
                .Where(d => d.SchoolId == schoolId && !d.IsDeleted)
                .ToListAsync(ct);
        }

        public async Task<List<int>> GetRegisteredDancerIdsForFeisAsync(int feisId, CancellationToken ct = default)
        {
            await using var db = _dbFactory.CreateDbContext();
            return await db.DancerRegistrations.AsNoTracking()
                .Where(dr => dr.FeisId == feisId && !dr.IsDeleted)
                .Select(dr => dr.DancerId)
                .ToListAsync(ct);
        }

        public async Task<Dancer?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using var db = _dbFactory.CreateDbContext();
            return await db.Dancers.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted, ct);
        }
    }
}
