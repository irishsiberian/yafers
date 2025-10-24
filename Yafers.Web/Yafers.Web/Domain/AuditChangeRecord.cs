namespace Yafers.Web.Domain
{
    public class AuditChangeRecord
    {
        public string FieldName { get; set; }
        public object? OldValue { get; set; }
        public object? NewValue { get; set; }
    }
}
