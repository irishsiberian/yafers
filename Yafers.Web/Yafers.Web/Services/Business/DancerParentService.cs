using Microsoft.EntityFrameworkCore;
using Yafers.Web.Data;
using Yafers.Web.Data.Entities;
using Yafers.Web.Services.Business.Interfaces;

namespace Yafers.Web.Services.Business
{
    public class DancerParentService : IDancerParentService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public DancerParentService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<DancerParent?> GetByUserAsync(string userId)
        {
            await using var db = _contextFactory.CreateDbContext();
            return await db.DancerParents.FirstOrDefaultAsync(d => d.UserId == userId);
        }
    }
}
