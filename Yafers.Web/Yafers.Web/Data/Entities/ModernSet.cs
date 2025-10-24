using Yafers.Web.Data.Entities.Enums;
using Yafers.Web.Data.Entities.Interfaces;

namespace Yafers.Web.Data.Entities
{
    public class ModernSet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ModernSetType Type { get; set; }
    }
}
