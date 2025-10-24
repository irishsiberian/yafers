using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Yafers.Web.Domain
{
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
            Changes = new List<AuditChangeRecord>();
            TemporaryProperties = new List<PropertyEntry>();
        }

        public EntityEntry Entry { get; }
        public string? TableName { get; set; }
        public int? LastModifiedById { get; set; }
        public int? AuditableObjectId { get; set; }
        public int? AuditableEntityType { get; set; }

        public List<AuditChangeRecord> Changes { get; }
        public List<PropertyEntry> TemporaryProperties { get; }

        public bool HasTemporaryProperties =>
            TemporaryProperties != null && TemporaryProperties.Any();

        public Data.Entities.Audit ToAudit()
        {
            var audit = new Data.Entities.Audit();
            audit.TableName = TableName;
            audit.CreatedDate = DateTime.UtcNow;
            audit.Changes = Changes.Count == 0 ? null : JsonSerializer.Serialize(Changes);
            audit.LastUpdatedById = LastModifiedById ?? 0;
            audit.AuditableObjectId = AuditableObjectId ?? 0;
            audit.AuditableEntityType = AuditableEntityType ?? 0;
            return audit;
        }
    }
}
