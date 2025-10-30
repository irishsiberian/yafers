using Yafers.Web.Data.Entities.Interfaces;

namespace Yafers.Web.Data.Entities
{
    public class DancerRegistration : IAuditable
    {
        public int Id { get; set; }
        public int DancerId { get; set; }
        public int FeisId { get; set; }
        public int DancerNumber { get; set; }
        public DateTime? NumberAssignedAtUtc { get; set; }
        public bool IsYafersFeePaid { get; set; }
        public DateTime? YafersFeePaidAtUtc { get; set; }
        public bool IsInCart { get; set; }
        public string? InCartForUser { get; set; }
        public DateTime? AddedToCartAtUtc { get; set; }
        //public string RegistrarId { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public string? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }

        public Feis Feis { get; set; }
        public List<CompetitionRegistration> CompetitionRegistrations { get; set; }
    }
}
