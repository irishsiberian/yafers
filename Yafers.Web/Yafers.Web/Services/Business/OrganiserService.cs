using Microsoft.EntityFrameworkCore;
using Yafers.Web.Data;
using Yafers.Web.Data.Entities;
using Yafers.Web.Services.Business.Interfaces;

namespace Yafers.Web.Services.Business
{
    public class OrganiserService : IOrganiserService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public OrganiserService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Organiser?> GetByUserAsync(string userId)
        {
            await using var db = _contextFactory.CreateDbContext();
            return await db.Organisers.FirstOrDefaultAsync(d => d.UserId == userId);
        }
    }
}
