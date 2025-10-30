using Yafers.Web.Data.Entities;
using Feis = Yafers.Web.Data.Entities.Feis;
namespace Yafers.Web.Services.Feiseanna
{
    public interface IFeisService
    {
        Task<List<Data.Entities.Feis>> GetAvailableFeiseannaAsync(CancellationToken ct = default);
        Task<bool> UserHasCartItemsAsync(string userId, CancellationToken ct = default);
        Task<Data.Entities.Feis?> GetFeisWithSyllabusAsync(int feisId, CancellationToken ct = default);
        Task<List<Competition>> GetCompetitionsForSyllabusAsync(int syllabusId, CancellationToken ct = default);
    }
}