using Microsoft.EntityFrameworkCore;
using Yafers.Web.Data;
using Yafers.Web.Data.Entities;
using Yafers.Web.Services.Business.Interfaces;

namespace Yafers.Web.Services.Business
{
    public class TeacherService : ITeacherService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public TeacherService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Teacher?> GetByUserAsync(string userId)
        {
            await using var db = _contextFactory.CreateDbContext();
            return await db.Teachers.FirstOrDefaultAsync(d => d.UserId == userId);
        }
    }
}
