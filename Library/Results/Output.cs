using System.ComponentModel.DataAnnotations;

namespace Library.Results
{
    /// <summary>
    /// Class expressing default api output contract to swagger docs.
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public abstract class Output<T>
    {
        public T Data { get; set; }
        [Required] public string[] Errors { get; set; }
    }
}
