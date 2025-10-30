namespace Yafers.Web.Services.Business.Interfaces
{
    public interface IRegistrationService
    {
        Task<int> CreateDancerRegistrationAsync(int dancerId, int feisId, string createdBy, CancellationToken ct = default);
        Task DeleteDancerRegistrationAsync(int dancerRegistrationId, CancellationToken ct = default);
        Task<bool> AddCompetitionRegistrationAsync(int dancerRegistrationId, int dancerId, int competitionId, int feisId, string registrarId, CancellationToken ct = default);
        Task RemoveCompetitionRegistrationAsync(int dancerRegistrationId, int competitionId, CancellationToken ct = default);
        Task<HashSet<int>> GetCompetitionRegistrationIdsAsync(int dancerRegistrationId, CancellationToken ct = default);
        Task<List<Data.Entities.DancerRegistration>> GetDancerRegistrationsInCartAsync(int feisId, string inCartForUser, CancellationToken ct = default);

    }
}
