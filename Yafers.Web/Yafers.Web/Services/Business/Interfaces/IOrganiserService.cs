using Yafers.Web.Data.Entities;

namespace Yafers.Web.Services.Business.Interfaces
{
    public interface IOrganiserService
    {
        Task<Organiser?> GetByUserAsync(string userId);
    }
}
