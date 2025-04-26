
using KrutayaReklama.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;


namespace KrutayaReklama.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPlatformService _platformService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IPlatformService service)
        {
            _logger = logger;
            _platformService = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UploadFile([FromForm] IFormFile file) 
        {
            if (!ModelState.IsValid)
                return ErrorBadRequest("Файл не правильный какой-то...");

            if (file == null || file.Length == 0)
                return ErrorBadRequest("Файл не загружен или пуст");
            try
            {
                _platformService.UploadFile(file);
                return Ok(new { message = "Файл успешно загружен" });
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Ошибка формата данных при загрузке файла");
                return ErrorBadRequest("Ошибка в формате файла. Проверьте правильность данных.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Необработанная ошибка при загрузке файла");
                return ErrorInternalServer("Произошла внутренняя ошибка сервера.");
            }
        }

        public IActionResult FindPlatforms(string location)
        {
            if (string.IsNullOrEmpty(location))
                return ErrorBadRequest("ты не передал локацию для поиска" );

            var locations = location.Split('/');
            string lastLocation = locations[locations.Length - 1];

            List<string> result = new List<string>();
            try
            {
                result = _platformService.FindPlatforms(lastLocation);
            }
            catch (Exception ex)
            { 
                return ErrorInternalServer( ex.Message );
            }

            return Ok(result);
        }

        protected BadRequestObjectResult ErrorBadRequest(string message)
        {
            return BadRequest(new { error = message });
        }
        protected NotFoundObjectResult ErrorNotFound(string message)
        {
            return NotFound(new { error = message });
        }
        protected ObjectResult ErrorInternalServer(string message)
        {
            return StatusCode(500, new { error = message });
        }
    }
}
