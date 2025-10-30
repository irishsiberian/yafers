using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yafers.Web.Consts;
using Yafers.Web.Data;
using Yafers.Web.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Yafers.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // List доступен всем авторизованным; Create/Update/Delete дополнительно помечены
    public class SchoolsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public SchoolsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET api/schools?search=abc
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SchoolDto>>> Get([FromQuery] string? search)
        {
            var q = _db.Schools.AsNoTracking().Where(s => !s.IsDeleted);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var t = search.Trim();
                q = q.Where(s => s.Name.Contains(t));
            }
            var list = await q.OrderBy(s => s.Name)
                              .Select(s => new SchoolDto { Id = s.Id, Name = s.Name, City = s.City, Country = s.Country })
                              .ToListAsync();
            return Ok(list);
        }

        // POST api/schools
        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult<SchoolDto>> Create([FromBody] SchoolCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required.");

            var userId = _userManager.GetUserId(User);
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
                CreatedBy = userId ?? ""
            };

            _db.Schools.Add(school);
            await _db.SaveChangesAsync();

            var res = new SchoolDto { Id = school.Id, Name = school.Name, City = school.City, Country = school.Country };
            return CreatedAtAction(nameof(GetById), new { id = school.Id }, res);
        }

        // GET api/schools/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SchoolDto>> GetById(int id)
        {
            var s = await _db.Schools.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (s == null) return NotFound();
            return new SchoolDto { Id = s.Id, Name = s.Name, City = s.City, Country = s.Country };
        }

        // PUT api/schools/{id}
        [HttpPut("{id:int}")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] SchoolUpdateDto dto)
        {
            var school = await _db.Schools.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (school == null) return NotFound();

            school.Name = dto.Name?.Trim() ?? school.Name;
            school.Country = dto.Country ?? school.Country;
            school.City = dto.City ?? school.City;
            school.Address = dto.Address ?? school.Address;
            school.AssociationId = dto.AssociationId ?? school.AssociationId;
            school.ParentId = dto.ParentId ?? school.ParentId;

            school.UpdatedAtUtc = DateTime.UtcNow;
            school.UpdatedBy = _userManager.GetUserId(User) ?? "";

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/schools/{id}  (soft delete)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var school = await _db.Schools.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (school == null) return NotFound();

            school.IsDeleted = true;
            school.DeletedAtUtc = DateTime.UtcNow;
            school.DeletedBy = _userManager.GetUserId(User) ?? "";

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DTOs
        public sealed class SchoolDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string? City { get; set; }
            public string? Country { get; set; }
        }

        public sealed class SchoolCreateDto
        {
            public string Name { get; set; } = "";
            public string? Country { get; set; }
            public string? City { get; set; }
            public string? Address { get; set; }
            public int AssociationId { get; set; }
            public int? ParentId { get; set; }
        }

        public sealed class SchoolUpdateDto
        {
            public string? Name { get; set; }
            public string? Country { get; set; }
            public string? City { get; set; }
            public string? Address { get; set; }
            public int? AssociationId { get; set; }
            public int? ParentId { get; set; }
        }
    }
}