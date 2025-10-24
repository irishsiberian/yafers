using Yafers.Web.Data.Entities.Interfaces;

namespace Yafers.Web.Data.Entities
{
    public class Round : IAuditable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RoundOrder { get; set; }
        public int DanceId { get; set; }
        public string Description { get; set; }
        public int CompetitionId { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public string DeletedBy { get; set; }
        public bool IsDeleted { get; set; }

        public Competition Competition { get; set; }

    }
}
