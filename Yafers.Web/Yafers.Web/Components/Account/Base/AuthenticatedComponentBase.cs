using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Yafers.Web.Data;
using Yafers.Web.Data.Entities;
using Yafers.Web.Services;

namespace Yafers.Web.Components.Account.Base
{
    public abstract class AuthenticatedComponentBase : ComponentBase
    {
        [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        [Inject] protected IDbContextFactory<ApplicationDbContext> DbFactory { get; set; } = default!;
        [Inject] protected IdentityRedirectManager RedirectManager { get; set; } = default!;
        [Inject] private IUserPermissionService UserPermissionService { get; set; } = default!;
        [Inject] protected NavigationManager _nav { get; set; } = default!;

        protected ClaimsPrincipal? CurrentPrincipal { get; private set; }
        protected ApplicationUser? CurrentUser { get; private set; }

        // one-time redirect target computed during initialization
        private string? _pendingRedirect;
        private bool _redirectPerformed;

        protected override async Task OnInitializedAsync()
        {
            var auth = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            CurrentPrincipal = auth.User;

            if (CurrentPrincipal?.Identity?.IsAuthenticated != true)
            {
                RedirectManager.RedirectTo(Consts.Routes.Login);
                return;
            }
            var id = CurrentPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await using var db = DbFactory.CreateDbContext();
            CurrentUser =  await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (CurrentUser == null)
            {
                RedirectManager.RedirectTo(Consts.Routes.Login);
                return;
            }

            _pendingRedirect = await UserPermissionService.GetNextIncompleteRolePageAsync(CurrentUser);

            await OnUserInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !_redirectPerformed && !string.IsNullOrEmpty(_pendingRedirect))
            {
                // guard: don't redirect if we're already on that page
                var relative = _nav.ToBaseRelativePath(_nav.Uri);
                if (!relative.Contains(_pendingRedirect.TrimStart('/'), StringComparison.OrdinalIgnoreCase))
                {
                    _redirectPerformed = true;
                    RedirectManager.RedirectTo(_pendingRedirect);
                }
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        // Для переопределения в дочерних компонентах
        protected virtual Task OnUserInitializedAsync() => Task.CompletedTask;
    }
}