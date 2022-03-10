using Microsoft.Extensions.Caching.Distributed;
using RedisSample.Repository;

namespace RedisSample.CronJob
{
    public class BackgroundAutomaticSaveColorDb : BackgroundService
    {
        private readonly IAutomaticSaveColorDb _automaticSaveColorDb;

        public BackgroundAutomaticSaveColorDb(IAutomaticSaveColorDb automaticSaveColorDb)
        {
            _automaticSaveColorDb = automaticSaveColorDb;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await _automaticSaveColorDb.RunColorSaveThread(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
        }
    }

    public class AutomaticSaveColorDb : IAutomaticSaveColorDb
    {
        private readonly IColorRepository _colorRepository;
        private readonly IDistributedCache _distributedCache;
        
        public AutomaticSaveColorDb(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            _colorRepository = new ColorRepository(_distributedCache);
        }

        public async Task RunColorSaveThread(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var color = await _colorRepository.GetColor();
                    if (color.UpToDate == false)
                    {
                        color.UpToDate = true;
                        await _colorRepository.SetColorDb(color);
                        await _colorRepository.SetColor(color);
                    }
                    await Task.Delay(20 * 1000, cancellationToken);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public interface IAutomaticSaveColorDb
    {
        Task RunColorSaveThread(CancellationToken cancellationToken);
    }
}
