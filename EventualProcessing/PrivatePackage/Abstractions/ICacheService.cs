using System.Threading.Tasks;

namespace PrivatePackage.Abstractions
{
    public interface ICacheService
    {
        Task<string> GetAsync(string key);
        Task SetAsync(string key, string value, int timeInSeconds);
    }
}
