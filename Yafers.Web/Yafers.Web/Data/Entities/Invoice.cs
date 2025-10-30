using Yafers.Web.Data.Entities.Interfaces;

namespace Yafers.Web.Data.Entities
{
    public class Invoice : IAuditable
    {
        public int Id { get; set; }
        public int FeisId { get; set; }
        public DateTime InvoicedAtUtc { get; set; }
        public DateTime? PaidAtUtc { get; set; }
        private string PaidBy { get; set; }
        public decimal TotalSum { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public string? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }

        public List<CompetitionRegistration> CompetitionRegistrations { get; set; }
        public Feis Feis { get; set; }
    }
}
