using System.Threading.Tasks;

namespace Library.Abstractions
{
    public interface ICacheService
    {
        Task<string> GetAsync(string key);
        Task SetAsync(string key, string value, int timeInSeconds);
    }
}
