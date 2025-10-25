using Yafers.Web.Data.Entities.Enums;
using Yafers.Web.Data.Entities.Interfaces;

namespace Yafers.Web.Data.Entities
{
    public class Competition : IAuditable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSolo { get; set; }
        public int? DanceId { get; set; }
        public DanceLevel Level { get; set; }
        public bool IsComplex { get; set; }
        public int Speed { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public bool IsTeam { get; set; }
        public TeamType TeamType { get; set; }
        public bool IsModernSet { get; set; }
        public bool IsSpecial { get; set; }
        public string Description { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public string DeletedBy { get; set; }
        public bool IsDeleted { get; set; }

        public List<Round> Rounds { get; set; }
        public Dance Dance { get; set; }
        public List<SyllabusCompetition> SyllabusCompetitions { get; set; }
        public List<Syllabus> Syllabi { get; set; }
    }
}
