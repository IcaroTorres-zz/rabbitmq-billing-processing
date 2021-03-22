using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PrivatePackage.Results
{
    /// <summary>
    /// Class expressing default api output contract to swagger docs.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ExcludeFromCodeCoverage]
    public abstract class Output<T>
    {
        public T Data { get; set; }
        [Required] public string[] Errors { get; set; }
    }
}
