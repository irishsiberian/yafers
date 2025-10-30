using Yafers.Web.Data.Entities;

namespace Yafers.Web.Services.Business.Interfaces
{
    public interface IDancerService
    {
        Task<Dancer?> GetByUserAsync(string userId);
    }
}
