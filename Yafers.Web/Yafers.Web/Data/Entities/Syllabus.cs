using Yafers.Web.Data.Entities.Interfaces;

namespace Yafers.Web.Data.Entities
{
    public class Syllabus : IAuditable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AssociationId { get; set; }
        public decimal AdminFee { get; set; }
        public bool IsTemplate { get; set; }
        public decimal SoloDancePrice { get; set; }
        public decimal PremiershipPrice { get; set; }
        public decimal ChampionshipPrice { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public string? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }

        public List<SyllabusCompetition> SyllabusCompetitions { get; set; }
        public List<Competition> Competitions { get; set; }
    }
}
