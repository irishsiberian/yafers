using Yafers.Web.Data.Entities.Interfaces;

namespace Yafers.Web.Data.Entities
{
    public class Organiser : IAuditable
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public bool StripeEnabled { get; set; }
        public string StripeKey { get; set; }
        public bool PayPalEnabled { get; set; }
        public string PayPalCode { get; set; }
        public bool IsApprovedByAdmin { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public string DeletedBy { get; set; }
        public bool IsDeleted { get; set; }

        public ApplicationUser OrganiserUser { get; set; }
    }
}
