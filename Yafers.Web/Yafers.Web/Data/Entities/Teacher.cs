using Yafers.Web.Data.Entities.Enums;
using Yafers.Web.Data.Entities.Interfaces;

namespace Yafers.Web.Data.Entities
{
    public class Teacher : IAuditable
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? SchoolId { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? UserId { get; set; }
        public TeacherQualification Qualification { get; set; }
        public bool IsApprovedByAdminOrAnotherTeacher { get; set; }
        public DateTime? ApprovedAtUtc { get; set; }
        public string? ApprovedBy { get; set; }


        public DateTime CreatedAtUtc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public string? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }

        public ApplicationUser? TeacherUser { get; set; }
        public School? School { get; set; }
        public List<AffiliationFee>? AffiliationFees { get; set; }
    }
}
