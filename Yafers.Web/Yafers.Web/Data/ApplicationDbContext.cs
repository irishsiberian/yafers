using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Yafers.Web.Entities;

namespace Yafers.Web.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
    {
        public DbSet<Dancer> Dancers { get; set; }

        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
    }
 
}
