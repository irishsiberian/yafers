using Yafers.Web.Data.Entities.Interfaces;

namespace Yafers.Web.Data.Entities
{
    public class SyllabusCompetition : IAuditable
    {
        public int Id { get; set; }
        public int CompetitionId { get; set; }
        public int SyllabusId { get; set; }
        public decimal PriceOverride { get; set; }
        public int RegistrationOrder { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public string? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }

        public Competition? Competition { get; set; }
        public Syllabus? Syllabus { get; set; }
    }
}
