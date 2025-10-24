namespace Yafers.Web.Data.Entities
{
    public class Audit
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Changes { get; set; }
        public int LastUpdatedById { get; set; }
        public int AuditableObjectId { get; set; }
        public int AuditableEntityType { get; set; }
    }
}
