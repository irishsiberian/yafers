using Yafers.Web.Data.Entities;

namespace Yafers.Web.Services.Business.Interfaces
{
    public interface IDancerParentService
    {
        Task<DancerParent?> GetByUserAsync(string userId);
    }
}
