using Yafers.Web.Data.Entities;

namespace Yafers.Web.Services.Business.Interfaces
{
    public interface ITeacherService
    {
        Task<Teacher?> GetByUserAsync(string userId);
    }
}
