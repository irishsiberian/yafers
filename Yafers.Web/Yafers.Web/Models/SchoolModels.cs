namespace Yafers.Web.Models
{
    public sealed class SchoolDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    public sealed class SchoolCreateDto
    {
        public string Name { get; set; } = "";
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public int AssociationId { get; set; }
        public int? ParentId { get; set; }
    }

    public sealed class SchoolUpdateDto
    {
        public string? Name { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public int? AssociationId { get; set; }
        public int? ParentId { get; set; }
    }
}