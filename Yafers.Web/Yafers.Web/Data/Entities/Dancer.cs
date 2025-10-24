using Yafers.Web.Data.Entities.Enums;
using Yafers.Web.Data.Entities.Interfaces;

namespace Yafers.Web.Data.Entities
{
    public class Dancer : IAuditable
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public int SchoolId { get; set; }
        public string? UserId { get; set; }
        public int? DancerParentId { get; set; }
        public string? DancerParentUserId { get; set; }
        public int PreliminaryWinCount { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public string DeletedBy { get; set; }
        public bool IsDeleted { get; set; }

        public School School { get; set; }
        public DancerParent DancerParent { get; set; }
        public ApplicationUser DancerUser { get; set; }
        public ApplicationUser DancerParentUser { get; set; }
        public List<DancerRegistration> Registrations { get; set; }
    }
}
