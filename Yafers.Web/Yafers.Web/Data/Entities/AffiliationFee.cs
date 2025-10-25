namespace Yafers.Web.Data.Entities
{
    public class AffiliationFee
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public int Year { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAtUtc { get; set; }
    }
}
