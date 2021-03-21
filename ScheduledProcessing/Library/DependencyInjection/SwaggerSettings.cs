using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public class SwaggerSettings
    {
        public string Title { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string Template { get; set; }
        public string Url { get; set; }
    }
}
