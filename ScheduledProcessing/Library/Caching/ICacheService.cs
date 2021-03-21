using System.Threading.Tasks;

namespace Library.Caching
{
    public interface ICacheService
    {
        Task<string> GetAsync(string key);
        Task<bool> SetAsync(string key, string value, int timeInSeconds);
    }
}
