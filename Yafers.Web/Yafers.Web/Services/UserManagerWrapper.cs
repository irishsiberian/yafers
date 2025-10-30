using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Yafers.Web.Data;
using Yafers.Web.Data.Entities;

namespace Yafers.Web.Services
{
    public class UserManagerWrapper
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IEnumerable<IUserValidator<ApplicationUser>> _userValidators;
        private readonly IEnumerable<IPasswordValidator<ApplicationUser>> _passwordValidators;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly IdentityErrorDescriber _errors;
        private readonly IServiceProvider _services;
        private readonly ILogger<UserManager<ApplicationUser>> _logger;

        public UserManagerWrapper(
            IDbContextFactory<ApplicationDbContext> contextFactory,
            IUserStore<ApplicationUser> userStore,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<ApplicationUser>> logger)
        {
            _contextFactory = contextFactory;
            _userStore = userStore;
            _passwordHasher = passwordHasher;
            _userValidators = userValidators;
            _passwordValidators = passwordValidators;
            _keyNormalizer = keyNormalizer;
            _errors = errors;
            _services = services;
            _logger = logger;
        }

        private UserManager<ApplicationUser> CreateUserManager(ApplicationDbContext context)
        {
            var userStore = new UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext>(context);

            return new UserManager<ApplicationUser>(
                userStore,
                null,
                _passwordHasher,
                _userValidators,
                _passwordValidators,
                _keyNormalizer,
                _errors,
                _services,
                _logger);
        }

        public async Task<ApplicationUser> GetUserAsync(string userId)
        {
            await using var context = _contextFactory.CreateDbContext();
            var userManager = CreateUserManager(context);
            return await userManager.FindByIdAsync(userId);
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
        {
            await using var context = _contextFactory.CreateDbContext();
            var userManager = CreateUserManager(context);
            return await userManager.IsInRoleAsync(user, role);
        }

        public async Task<IdentityResult> AddToRoleAsync(ClaimsPrincipal principal, string role)
        {
            await using var context = _contextFactory.CreateDbContext();
            var userManager = CreateUserManager(context);
            var user = await userManager.GetUserAsync(principal);
            return await userManager.AddToRoleAsync(user, role);
        }

        public async Task<ApplicationUser> GetUserAsync(ClaimsPrincipal principal)
        {
            await using var context = _contextFactory.CreateDbContext();
            var userManager = CreateUserManager(context);
            return await userManager.GetUserAsync(principal);
        }

        public async Task<ApplicationUser> GetUserNoTrackingAsync(ClaimsPrincipal principal)
        {
            await using var context = _contextFactory.CreateDbContext();
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
            return user;
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            await using var context = _contextFactory.CreateDbContext();
            var userManager = CreateUserManager(context);
            return await userManager.GetRolesAsync(user);
        }
    }

}
