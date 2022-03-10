using Microsoft.AspNetCore.Mvc;
using RedisSample.Models;
using RedisSample.Repository;
using System.Diagnostics;

namespace RedisSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IColorRepository _colorRepository;

        public HomeController(ILogger<HomeController> logger, IColorRepository colorRepository)
        {
            _logger = logger;
            _colorRepository = colorRepository;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                //string colorValue = "omega fatty acid";
                //byte[] colorByte = Encoding.UTF8.GetBytes(colorValue);
                //await _distributedCache.SetAsync(cacheKey, colorByte, new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddDays(2)});
                var redisColorList = await _colorRepository.GetColor();
                string colorList = String.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return View();
        }

        public async Task<IActionResult> Set()
        {
            //var color = "get to the new color set";
            //await _colorRepository.SetColor(color);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Set(Color color)
        {
            try
            {
                //var color = "get to the new color set";
                color.UpToDate = false;
                await _colorRepository.SetColor(color);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return RedirectToAction(nameof(Get));
        }

        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _colorRepository.GetColor();
                //Color color = new Color() { ColorName = result };

                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return Error();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}