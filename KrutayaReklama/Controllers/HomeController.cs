
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
                return ErrorBadRequest("���� �� ���������� �����-��...");

            if (file == null || file.Length == 0)
                return ErrorBadRequest("���� �� �������� ��� ����");
            try
            {
                _platformService.UploadFile(file);
                return Ok(new { message = "���� ������� ��������" });
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "������ ������� ������ ��� �������� �����");
                return ErrorBadRequest("������ � ������� �����. ��������� ������������ ������.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "�������������� ������ ��� �������� �����");
                return ErrorInternalServer("��������� ���������� ������ �������.");
            }
        }

        public IActionResult FindPlatforms(string location)
        {
            if (string.IsNullOrEmpty(location))
                return ErrorBadRequest("�� �� ������� ������� ��� ������" );

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
