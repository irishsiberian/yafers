using Yafers.Web.Data.Entities;

namespace Yafers.Web.Services.Business.Interfaces
{
    public interface IDancerService
    {
        Task<Dancer?> GetByUserAsync(string userId);

        Task<List<Dancer>> GetDancersForUserAsDancerAsync(string userId, CancellationToken ct = default);
        Task<List<Dancer>> GetDancersForParentAsync(string parentUserId, CancellationToken ct = default);
        Task<List<Dancer>> GetDancersForTeacherSchoolAsync(int? schoolId, CancellationToken ct = default);
        Task<List<int>> GetRegisteredDancerIdsForFeisAsync(int feisId, CancellationToken ct = default);
        Task<Dancer?> GetByIdAsync(int id, CancellationToken ct = default);
    }

}