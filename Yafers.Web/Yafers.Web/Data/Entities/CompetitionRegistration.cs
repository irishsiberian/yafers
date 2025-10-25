using Yafers.Web.Data.Entities.Interfaces;

namespace Yafers.Web.Data.Entities
{
    public class CompetitionRegistration : IAuditable
    {
        public int Id { get; set; }
        public int DancerId { get; set; }
        public int? DancerRegistrationId { get; set; }
        public int FeisId { get; set; }
        public int CompetitionId { get; set; }
        public int? InvoiceId { get; set; }
        public int? TeamId { get; set; }
        public int? ModernSetId { get; set; }
        public int? ModernSetSpeed { get; set; }
        public int? CeiliId { get; set; }
        public string RegistrarId { get; set; }
        public bool IsInCart { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public string DeletedBy { get; set; }
        public bool IsDeleted { get; set; }

        public Dancer Dancer { get; set; }
        public DancerRegistration DancerRegistration { get; set; }
        public ApplicationUser Registrar { get; set; }
        public Competition Competition { get; set; }
        public Feis Feis { get; set; }
        public Invoice Invoice { get; set; }
        public Team Team { get; set; }
    }
}
