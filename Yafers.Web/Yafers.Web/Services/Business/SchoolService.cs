using Microsoft.EntityFrameworkCore;
using Yafers.Web.Data;
using Yafers.Web.Data.Entities;
using Yafers.Web.Models;
using Yafers.Web.Services.Business.Interfaces;

namespace Yafers.Web.Services.Business
{
    public class SchoolService : ISchoolService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public SchoolService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<SchoolDto>> SearchAsync(string? search, CancellationToken ct = default)
        {
            using var db = _contextFactory.CreateDbContext();
            var q = db.Schools.AsNoTracking().Where(s => !s.IsDeleted);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var t = search.Trim();
                q = q.Where(s => s.Name.Contains(t));
            }

            return await q.OrderBy(s => s.Name)
                          .Select(s => new SchoolDto { Id = s.Id, Name = s.Name, City = s.City, Country = s.Country })
                          .ToListAsync(ct);
        }

        public async Task<SchoolDto?> GetByIdAsync(int? id, CancellationToken ct = default)
        {
            if (id == null)
                return null;
            using var db = _contextFactory.CreateDbContext();
            var s = await db.Schools.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id.Value && !x.IsDeleted, ct);
            if (s == null) return null;
            return new SchoolDto { Id = s.Id, Name = s.Name, City = s.City, Country = s.Country };
        }

        public async Task<SchoolDto> CreateAsync(SchoolCreateDto dto, string createdBy, CancellationToken ct = default)
        {
            using var db = _contextFactory.CreateDbContext();
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name is required.", nameof(dto));

            var now = DateTime.UtcNow;
            var school = new School
            {
                Name = dto.Name.Trim(),
                Country = dto.Country ?? "",
                City = dto.City ?? "",
                Address = dto.Address ?? "",
                AssociationId = dto.AssociationId,
                ParentId = dto.ParentId,
                CreatedAtUtc = now,
                CreatedBy = createdBy ?? ""
            };

            db.Schools.Add(school);
            await db.SaveChangesAsync(ct);

            return new SchoolDto { Id = school.Id, Name = school.Name, City = school.City, Country = school.Country };
        }

        public async Task<bool> UpdateAsync(int id, SchoolUpdateDto dto, string updatedBy, CancellationToken ct = default)
        {
            using var db = _contextFactory.CreateDbContext();
            var school = await db.Schools.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
            if (school == null) return false;

            school.Name = dto.Name?.Trim() ?? school.Name;
            school.Country = dto.Country ?? school.Country;
            school.City = dto.City ?? school.City;
            school.Address = dto.Address ?? school.Address;
            school.AssociationId = dto.AssociationId ?? school.AssociationId;
            school.ParentId = dto.ParentId ?? school.ParentId;
            school.UpdatedAtUtc = DateTime.UtcNow;
            school.UpdatedBy = updatedBy ?? "";

            await db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string deletedBy, CancellationToken ct = default)
        {
            using var db = _contextFactory.CreateDbContext();
            var school = await db.Schools.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
            if (school == null) return false;

            school.IsDeleted = true;
            school.DeletedAtUtc = DateTime.UtcNow;
            school.DeletedBy = deletedBy ?? "";

            await db.SaveChangesAsync(ct);
            return true;
        }
    }
}