using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;
using Yafers.Web.Data.Entities;
using Yafers.Web.Data.Entities.Enums;
using Yafers.Web.Data.Entities.Interfaces;
using Yafers.Web.Domain;
using Yafers.Web.Middleware;

namespace Yafers.Web.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILogger<ApplicationDbContext> logger, bool isSystemAudit = false) : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
    {
        private static readonly object _consoleLock = new();
        private readonly Guid _instanceId = Guid.NewGuid();
        private const string LastUpdatedByIdField = "LastUpdatedById";
        private const string SuperUserId = "SuperUserId";
        private readonly Dictionary<Type, HashSet<string>> _propertiesToSkip = new Dictionary<Type, HashSet<string>>();

        private void Log(string message)
        {
            try
            {
                lock (_consoleLock)
                {
                    var msg = $"[DbContext:{_instanceId}] [Thread:{Thread.CurrentThread.ManagedThreadId}] {message}";
                    logger.LogInformation(msg);
                    Console.WriteLine(msg);
                }
            }
            catch
            {
                // ignore logging errors
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Log("OnModelCreating");
            base.OnModelCreating(modelBuilder);
            var configurator = new EntityConfigurator();

            configurator.Configure(modelBuilder);
        }

       public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default
        )
        {
            Log("SaveChangesAsync ENTER");
            var auditEntries = OnBeforeSaveChanges();
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            OnAfterSaveChanges(auditEntries);
            Log("SaveChangesAsync AFTER first base.SaveChangesAsync");
            await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            Log("SaveChangesAsync EXIT");
            return result;
        }

        public override int SaveChanges(
            bool acceptAllChangesOnSuccess
        )
        {
            Log("SaveChanges ENTER");
            var auditEntries = OnBeforeSaveChanges();
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            OnAfterSaveChanges(auditEntries);
            base.SaveChanges(acceptAllChangesOnSuccess);
            Log("SaveChanges EXIT");
            return result;
        }

        private List<AuditEntry> OnBeforeSaveChanges()
        {
            Log("OnBeforeSaveChanges START");
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries<IAuditable>())
            {
                HandleEntry(entry, auditEntries);
            }

            // Save audit entities that have all the modifications
            foreach (var auditEntry in auditEntries)
            {
                if (auditEntry.Changes.Count == 0)
                    continue;
                Audits.Add(auditEntry.ToAudit());
            }

            // Keep a list of entries where the value of some properties are unknown at this step
            Log("OnBeforeSaveChanges END");
            return auditEntries.Where(e => e.HasTemporaryProperties).ToList();
        }

        private void HandleEntry(
            EntityEntry<IAuditable> entry,
            List<AuditEntry> auditEntries
        )
        {
            if (
                entry.State == EntityState.Detached
                || entry.State == EntityState.Unchanged
            )
                return;

            IEnumerable<PropertyEntry> props = entry.Properties.ToList();
            PropertyEntry? lastModifiedById = props.FirstOrDefault(
                p => p.Metadata.Name == LastUpdatedByIdField
            );

            //Modified by system account
            if (isSystemAudit && lastModifiedById is { IsModified: false })
            {
                entry.Entity.UpdatedBy = SuperUserId;
            }

            var auditEntry = new AuditEntry(entry);
            var tableName = entry.Metadata.GetTableName();
            auditEntry.TableName = tableName;
            if (Enum.TryParse<AuditableEntityType>(tableName, out var entryType))
            {
                auditEntry.AuditableEntityType = (int)entryType;
            }

            auditEntries.Add(auditEntry);
            PropertyValues? databaseValues = entry.GetDatabaseValues();

            int? lastModifiedIdValue = null;
            if (null != lastModifiedById)
            {
                lastModifiedIdValue = lastModifiedById.CurrentValue as int?;
            }
            if (lastModifiedIdValue == null)
            {
                var lastModifiedByProperty = entry.Entity.GetType().GetProperty(
                    "Domain.Models.Interfaces.IAuditable." + LastUpdatedByIdField,
                    BindingFlags.NonPublic | BindingFlags.Instance
                );
                if (lastModifiedByProperty != null)
                {
                    lastModifiedIdValue = lastModifiedByProperty.GetValue(entry.Entity) as int?;
                }
            }
            auditEntry.LastModifiedById = lastModifiedIdValue;

            foreach (var property in props)
            {
                if (_propertiesToSkip.TryGetValue(entry.Entity.GetType(), out var p) && p.Contains(property.Metadata.Name))
                {
                    continue;
                }

                if (databaseValues != null)
                {
                    GetChanges(property, databaseValues, auditEntry, entry);
                }
            }
        }

        private static void GetChanges(
            PropertyEntry property,
            PropertyValues? databaseValues,
            AuditEntry auditEntry,
            EntityEntry<IAuditable> entry
        )
        {
            string propertyName = property.Metadata.Name;
            var change = new AuditChangeRecord() { FieldName = propertyName };

            var originalValue = databaseValues?.GetValue<object>(propertyName);
            if (property.IsTemporary)
            {
                // value will be generated by the database, get the value after saving
                auditEntry.TemporaryProperties.Add(property);
                return;
            }

            if (property.Metadata.IsPrimaryKey())
            {
                change.NewValue = property.CurrentValue;
                auditEntry.AuditableObjectId =
                    property.CurrentValue != null ? (int)property.CurrentValue : 0;
                //auditEntry.Changes.Add(change); we don't need PK as a change, it will always create an audit record of even no other properties modified
                return;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    change.NewValue = property.CurrentValue;
                    auditEntry.Changes.Add(change);
                    break;

                case EntityState.Deleted:
                    change.OldValue = originalValue;
                    auditEntry.Changes.Add(change);
                    break;

                case EntityState.Modified:
                    HandleModified(property, originalValue, change, auditEntry);
                    break;
            }
        }

        private static void HandleModified(
            PropertyEntry property,
            object? originalValue,
            AuditChangeRecord change,
            AuditEntry auditEntry
        )
        {
            var trackChange = false;
            if (property.IsModified)
            {
                if (property.CurrentValue == null)
                {
                    if (originalValue != null)
                    {
                        trackChange = true;
                    }
                }
                else
                {
                    trackChange = !property.CurrentValue.Equals(originalValue);
                }
            }

            if (trackChange)
            {
                change.OldValue = originalValue;
                change.NewValue = property.CurrentValue;
                auditEntry.Changes.Add(change);
            }
        }

        private void OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries.Count == 0)
            {
                return;
            }

            foreach (var auditEntry in auditEntries)
            {
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    var propName = prop.Metadata.Name;
                    var change = new AuditChangeRecord() { FieldName = propName };
                    if (!prop.Metadata.IsPrimaryKey())
                    {
                        change.NewValue = prop.CurrentValue;
                        auditEntry.Changes.Add(change);
                    }
                    else
                    {
                        auditEntry.AuditableObjectId =
                            prop.CurrentValue != null ? (int)prop.CurrentValue : 0;
                    }
                }

                Audits.Add(auditEntry.ToAudit());
            }
        }

        public DbSet<AffiliationFee> AffiliationFees { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<Association> Associations { get; set; }
        public DbSet<Ceili> Ceilis { get; set; }
        public DbSet<Competition> Competitions { get; set; }
        public DbSet<CompetitionRegistration> CompetitionRegistrations { get; set; }
        public DbSet<Dance> Dances { get; set; }
        public DbSet<Dancer> Dancers { get; set; }
        public DbSet<DancerLevel> DancerLevels { get; set; }
        public DbSet<DancerParent> DancerParents { get; set; }
        public DbSet<DancerRegistration> DancerRegistrations { get; set; }
        public DbSet<Feis> Feiseanna { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<ModernSet> ModernSets { get; set; }
        public DbSet<Organiser> Organisers { get; set; }
        public DbSet<Feis> Reports { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Syllabus> Syllabi { get; set; }
        public DbSet<SyllabusCompetition> SyllabusCompetitions { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<AgeCategory> AgeCategories { get; set; }
    }
 
}
