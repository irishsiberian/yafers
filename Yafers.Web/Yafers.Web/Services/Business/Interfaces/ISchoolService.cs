using Yafers.Web.Models;

namespace Yafers.Web.Services.Business.Interfaces
{
    public interface ISchoolService
    {
        Task<List<SchoolDto>> SearchAsync(string? search, CancellationToken ct = default);
        Task<SchoolDto?> GetByIdAsync(int? id, CancellationToken ct = default);
        Task<SchoolDto> CreateAsync(SchoolCreateDto dto, string createdBy, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, SchoolUpdateDto dto, string updatedBy, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, string deletedBy, CancellationToken ct = default);
    }
}