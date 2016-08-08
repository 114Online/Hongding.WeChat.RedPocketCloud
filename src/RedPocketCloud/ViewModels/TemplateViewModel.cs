using RedPocketCloud.Models;

namespace RedPocketCloud.ViewModels
{
    public class TemplateViewModel
    {
        public TemplateType Type { get; set; }

        public long? Top { get; set; }

        public long? Bottom { get; set; }

        public long? Background { get; set; }

        public long? Drawn { get; set; }

        public long? Undrawn { get; set; }

        public long? Pending { get; set; }
    }
}
