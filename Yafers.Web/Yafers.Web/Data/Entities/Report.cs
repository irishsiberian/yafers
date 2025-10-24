using Yafers.Web.Data.Entities.Enums;

namespace Yafers.Web.Data.Entities
{
    public class Report
    {
        public int Id { get; set; }
        public int FeisId { get; set; }
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public ReportType Type { get; set; }
        public ReportStatus Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
