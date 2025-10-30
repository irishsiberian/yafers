using Yafers.Web.Data.Entities.Enums;
using Yafers.Web.Data.Entities.Interfaces;

namespace Yafers.Web.Data.Entities
{
    public class Feis : IAuditable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime FeisDate { get; set; }
        public DateTime RegistrationOpenDate { get; set; }
        public DateTime RegistrationCloseDate { get; set; }
        public string Location { get; set; }
        public string Venue { get; set; }
        public int OrganiserSchoolId { get; set; }
        public string Contacts { get; set; }
        public string EventUrl { get; set; }
        public string Description { get; set; }
        public FeisType FeisType { get; set; }
        public int AssociationId { get; set; }
        public int SyllabusId { get; set; }
        public int MaxEntriesCount { get; set; }
        public FeisStatus Status { get; set; }
        public bool IsAssociationFeePaid { get; set; }
        public DateTime? AssociationFeePaidAtUtc { get; set; }
        public bool IsCashPaymentAllowed { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public string DeletedBy { get; set; }
        public bool IsDeleted { get; set; }

        public Syllabus Syllabus { get; set; }
        public School OrganiserSchool { get; set; }
        public Association Association { get; set; }
        public List<Invoice> Invoices { get; set; }
        public List<CompetitionRegistration> CompetitionRegistrations { get; set; }
        public List<DancerRegistration> DancerRegistrations { get; set; }
        public List<Report> Reports { get; set; }
    }
}
