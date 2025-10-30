using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Yafers.Web.Consts;
using Yafers.Web.Data;
using Yafers.Web.Data.Entities;

namespace Yafers.Web.Services
{
    public interface IUserPermissionService
    {
        Task<bool> IsDancerAsync();
        Task<bool> IsDancerParentAsync();
        Task<bool> IsTeacherAsync();
        Task<bool> IsOrganiserAsync();
        Task<bool> IsAdminAsync();
        Task<bool> IsInRoleAsync(string roleName);
        Task<IdentityResult> AddToRoleAsync(string roleName);
        Task<bool> IsAuthenticatedAsync();
        Task<bool> CanRegisterToFeisAsync();
        Task<bool> CanManageStudentsAsync();
        Task<bool> CanManageSchoolAsync();
        Task<bool> CanManageFeisAsync();
        Task CreatePlaceholderEntity(List<string> roles);
        Task<string?> GetNextIncompleteRolePageAsync(ApplicationUser user);
        Task<ApplicationUser> GetCurrentUserAsync();
        Task<ApplicationUser> GetCurrentUserNoTrackingAsync();
        //Task<bool> EnsureProfileCompleteAsync(ApplicationUser user);
    }

    public class UserPermissionService : IUserPermissionService
    {
        private readonly AuthenticationStateProvider _authProvider;
        private readonly UserManagerWrapper _userManagerWrapper;
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly NavigationManager _nav;

        public UserPermissionService(
            AuthenticationStateProvider authProvider,
            UserManagerWrapper userManagerWrapper,
            IDbContextFactory<ApplicationDbContext> dbFactory,
            NavigationManager nav)
        {
            _authProvider = authProvider;
            _userManagerWrapper = userManagerWrapper;
            _dbFactory = dbFactory;
            _nav = nav;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var state = await _authProvider.GetAuthenticationStateAsync();
            return state.User?.Identity?.IsAuthenticated == true;
        }

        public async Task<bool> CanRegisterToFeisAsync()
        {
            var user = await GetCurrentUserNoTrackingAsync();
            if (user == null) return false;

            var roles = await _userManagerWrapper.GetRolesAsync(user);

            if (roles.Contains(RoleNames.Admin))
                return true;

            var profileCompleted = await IsProfileCompleteAsync(user, roles);
            if (!profileCompleted)
                return false;

            if (roles.Contains(RoleNames.Dancer) ||
                roles.Contains(RoleNames.DancerParent) ||
                roles.Contains(RoleNames.Teacher) ||
                roles.Contains(RoleNames.Organiser))
                return true;

            return false;
        }

        public async Task<bool> CanManageStudentsAsync()
        {
            var user = await GetCurrentUserNoTrackingAsync();
            if (user == null) return false;

            var roles = await _userManagerWrapper.GetRolesAsync(user);

            if (roles.Contains(RoleNames.Admin))
                return true;

            var profileCompleted = await IsProfileCompleteAsync(user, roles);
            if (!profileCompleted)
                return false;

            var isApprovedTeacher = false;
            if (roles.Contains(RoleNames.Teacher))
            {
                using var db = _dbFactory.CreateDbContext();
                var teacher = db.Teachers.FirstOrDefault(x => x.UserId == user.Id);
                if (teacher != null && teacher.IsApprovedByAdminOrAnotherTeacher)
                    isApprovedTeacher = true;
            }

            if (roles.Contains(RoleNames.DancerParent) ||
                isApprovedTeacher)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> CanManageSchoolAsync()
        {
            var user = await GetCurrentUserNoTrackingAsync();
            if (user == null) return false;

            var roles = await _userManagerWrapper.GetRolesAsync(user);

            if (roles.Contains(RoleNames.Admin))
                return true;

            var profileCompleted = await IsProfileCompleteAsync(user, roles);
            if (!profileCompleted)
                return false;

            if (roles.Contains(RoleNames.Teacher))
                return true;

            return false;
        }

        public async Task<bool> CanManageFeisAsync()
        {
            var user = await GetCurrentUserNoTrackingAsync();
            if (user == null) return false;

            var roles = await _userManagerWrapper.GetRolesAsync(user);

            if (roles.Contains(RoleNames.Admin))
                return true;

            var profileCompleted = await IsProfileCompleteAsync(user, roles);
            if (!profileCompleted)
                return false;

            var isApprovedOrganiser = false;
            if (roles.Contains(RoleNames.Teacher))
            {
                using var db = _dbFactory.CreateDbContext();
                var organiser = db.Organisers.FirstOrDefault(x => x.UserId == user.Id);
                if (organiser != null && organiser.IsApprovedByAdmin)
                    isApprovedOrganiser = true;
            }

            if (roles.Contains(RoleNames.Organiser))
                return true;

            return false;
        }

        private async Task<bool> IsProfileCompleteAsync(ApplicationUser user, IList<string> roles)
        {
            using var db = _dbFactory.CreateDbContext();

            if (roles.Contains(RoleNames.Admin))
                return true;

            if (roles.Contains(RoleNames.Dancer))
            {
                var dancer = await db.Dancers.FirstOrDefaultAsync(d => d.UserId == user.Id);
                if (dancer != null && !string.IsNullOrEmpty(dancer.FirstName) && !string.IsNullOrEmpty(dancer.LastName))
                    return true;
            }

            if (roles.Contains(RoleNames.DancerParent))
            {
                var parent = await db.DancerParents.FirstOrDefaultAsync(p => p.UserId == user.Id);
                if (parent != null && !string.IsNullOrEmpty(parent.FirstName) && !string.IsNullOrEmpty(parent.LastName))
                    return true;
            }

            if (roles.Contains(RoleNames.Teacher))
            {
                var teacher = await db.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);
                if (teacher != null && teacher.SchoolId != 0)
                    return true;
            }

            if (roles.Contains(RoleNames.Organiser))
            {
                var organiser = await db.Organisers.FirstOrDefaultAsync(o => o.UserId == user.Id);
                if (organiser != null)
                    return true;
            }

            return false;
        }
        
        public async Task<bool> IsDancerAsync()
        {
            return await IsInRoleAsync(RoleNames.Dancer);
        }

        public async Task<bool> IsDancerParentAsync()
        {
            return await IsInRoleAsync(RoleNames.DancerParent);
        }

        public async Task<bool> IsTeacherAsync()
        {
            return await IsInRoleAsync(RoleNames.Teacher);
        }

        public async Task<bool> IsOrganiserAsync()
        {
            return await IsInRoleAsync(RoleNames.Organiser);
        }

        public async Task<bool> IsAdminAsync()
        {
            return await IsInRoleAsync(RoleNames.Admin);
        }

        public async Task<ApplicationUser> GetCurrentUserNoTrackingAsync()
        {
            var authState = await _authProvider.GetAuthenticationStateAsync();
            var user = await _userManagerWrapper.GetUserNoTrackingAsync(authState.User);
            return user;
        }

        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var user = await _userManagerWrapper.GetUserAsync(_authProvider.GetAuthenticationStateAsync().Result.User);
            return user;
        }

        public async Task<bool> IsInRoleAsync(string roleName)
        {
            var user = await GetCurrentUserNoTrackingAsync();
            if (user == null) 
                return false;
            return await _userManagerWrapper.IsInRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> AddToRoleAsync(string roleName)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return IdentityResult.Failed();
            var authState = await _authProvider.GetAuthenticationStateAsync();
            return await _userManagerWrapper.AddToRoleAsync(authState.User, roleName);
        }

        public async Task CreatePlaceholderEntity(List<string> roles)
        {
            var user = await GetCurrentUserNoTrackingAsync();
            await using var db = _dbFactory.CreateDbContext();
            foreach (var role in roles)
            {

                // create placeholder entities if needed
                if (role == RoleNames.Dancer)
                {
                    if (!db.Dancers.Any(d => d.UserId == user.Id))
                    {
                        db.Dancers.Add(new Dancer
                        {
                            UserId = user.Id, FirstName = "", LastName = "", BirthDate = DateTime.UtcNow, SchoolId = null,
                            CreatedAtUtc = DateTime.UtcNow, CreatedBy = user.Id
                        });
                    }
                }

                if (role == RoleNames.DancerParent)
                {
                    if (!db.DancerParents.Any(p => p.UserId == user.Id))
                    {
                        db.DancerParents.Add(new DancerParent
                        {
                            UserId = user.Id, FirstName = "", LastName = "", Email = user.Email ?? "", Phone = "",
                            DancerIds = "", CreatedAtUtc = DateTime.UtcNow, CreatedBy = user.Id
                        });
                    }
                }

                if (role == RoleNames.Teacher)
                {
                    if (!db.Teachers.Any(t => t.UserId == user.Id))
                    {
                        db.Teachers.Add(new Teacher
                        {
                            UserId = user.Id, FirstName = "", LastName = "", SchoolId = null, Email = user.Email ?? "",
                            Phone = "", IsApprovedByAdminOrAnotherTeacher = false, CreatedAtUtc = DateTime.UtcNow,
                            CreatedBy = user.Id
                        });
                    }
                }

                if (role == RoleNames.Organiser)
                {
                    if (!db.Organisers.Any(o => o.UserId == user.Id))
                    {
                        db.Organisers.Add(new Organiser
                        {
                            UserId = user.Id, StripeEnabled = false, PayPalEnabled = false, IsApprovedByAdmin = false,
                            CreatedAtUtc = DateTime.UtcNow, CreatedBy = user.Id
                        });
                    }
                }
            }

            await db.SaveChangesAsync();
        }

        public async Task<string?> GetNextIncompleteRolePageAsync(ApplicationUser user)
        {
            var roles = await _userManagerWrapper.GetRolesAsync(user);

            if (roles.Contains(RoleNames.Admin))
                return "/";

            // Если нет выбранных ролей — перенаправляем на выбор ролей
            if (!roles.Any(r =>
                    r == RoleNames.Dancer ||
                    r == RoleNames.DancerParent ||
                    r == RoleNames.Teacher ||
                    r == RoleNames.Organiser))
            {
                return Routes.ChooseRoles;
            }

            await using var db = _dbFactory.CreateDbContext();

            if (await _userManagerWrapper.IsInRoleAsync(user, RoleNames.Dancer))
            {
                var dancer = await db.Dancers.FirstOrDefaultAsync(d => d.UserId == user.Id);
                if (dancer == null || string.IsNullOrEmpty(dancer.FirstName) || string.IsNullOrEmpty(dancer.LastName))
                    return Routes.CompleteDancer;
            }
            if (await _userManagerWrapper.IsInRoleAsync(user, RoleNames.DancerParent))
            {
                var parent = await db.DancerParents.FirstOrDefaultAsync(p => p.UserId == user.Id);
                if (parent == null || string.IsNullOrEmpty(parent.FirstName) || string.IsNullOrEmpty(parent.LastName))
                    return Routes.CompleteParent;
            }
            if (await _userManagerWrapper.IsInRoleAsync(user, RoleNames.Teacher))
            {
                var teacher = await db.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);
                if (teacher == null || teacher.SchoolId == 0 || string.IsNullOrEmpty(teacher.FirstName) || string.IsNullOrEmpty(teacher.LastName))
                    return Routes.CompleteTeacher;
            }
            if (await _userManagerWrapper.IsInRoleAsync(user, RoleNames.Organiser))
            {
                var organiser = await db.Organisers.FirstOrDefaultAsync(o => o.UserId == user.Id);
                if (organiser == null || (string.IsNullOrEmpty(organiser.StripeKey) && string.IsNullOrEmpty(organiser.PayPalCode)))
                    return Routes.CompleteOrganiser;
            }
            return null;
        }
    }
}