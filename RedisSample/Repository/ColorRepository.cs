using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RedisSample.Constants;
using RedisSample.Models;
using System.Text;

namespace RedisSample.Repository
{
    public class ColorRepository : IColorRepository
    {
        private AppDbContext _context;
        private readonly IDistributedCache _distributedCache;
        public ColorRepository(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            _context = new AppDbContext();
        }

        public async Task SetColor(Color color)
        {
            try
            {
                var serializeObj = JsonConvert.SerializeObject(color);
                byte[] colorByte = Encoding.UTF8.GetBytes(serializeObj);
                await _distributedCache.SetAsync(AppConstants.ColorKey, colorByte, new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddDays(2) });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SetColorDb(Color color)
        {
            try
            {
                await _context.Color.AddAsync(color);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Color> GetColor()
        {
            try
            {
                var result = await _distributedCache.GetStringAsync(AppConstants.ColorKey);

                return JsonConvert.DeserializeObject<Color>(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public interface IColorRepository
    {
        Task SetColor(Color color);
        Task<Color> GetColor();
        Task SetColorDb(Color color);
    }
}
