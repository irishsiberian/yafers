namespace Yafers.Web.Data.Entities
{
    public class Association
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal AffiliationFeeAmount { get; set; }
        public decimal LocalFeisFeeAmount { get; set; }
        public string StripeKey { get; set; }
    }
}
