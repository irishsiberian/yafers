using Microsoft.EntityFrameworkCore;
using Yafers.Web.Data;
using Yafers.Web.Data.Entities;
using Yafers.Web.Services.Business.Interfaces;

namespace Yafers.Web.Services.Business
{
    public class DancerService : IDancerService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public DancerService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Dancer?> GetByUserAsync(string userId)
        {
            await using var db = _contextFactory.CreateDbContext();
            return await db.Dancers.FirstOrDefaultAsync(d => d.UserId == userId);
        }
    }
}
