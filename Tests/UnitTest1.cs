using Moq;
using KrutayaReklama;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using KrutayaReklama.Services;
using KrutayaReklama.Controllers;
using Microsoft.AspNetCore.Http;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.IO;


namespace Tests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("Яндекс.Директ:/ru\r\nКрутая реклама:/ru/svrd", 200, "Файл успешно загружен")] 
        [InlineData("", 400, "Файл не загружен")] 
        [InlineData("Некорректная строка без двоеточия", 500, "Произошла внутренняя ошибка сервера.")] 

        public void UploadFile_All(string fileContent, int expectedStatusCode, string expectedMessage)
        {
            var mok = new Mock<IFormFile>();
            
            var fileStream = new MemoryStream();
            var writer = new StreamWriter(fileStream);
            writer.Write(fileContent);
            writer.Flush();
            fileStream.Position = 0;
            mok.Setup(f => f.OpenReadStream()).Returns(fileStream);
            mok.Setup(f => f.Length).Returns(fileStream.Length);

            var mokLogger = new Mock <ILogger<PlatformService>>();
            IPlatformService service = new PlatformService(mokLogger.Object);
            var mokControllerLogger = new Mock<ILogger<HomeController>>();

            var controller = new HomeController(mokControllerLogger.Object, service);
            var result = controller.UploadFile(mok.Object);

            if (expectedStatusCode == 400)
            {
                var badRequest = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal(expectedStatusCode, badRequest.StatusCode);
                Assert.Contains(expectedMessage, badRequest.Value.ToString());
            }
            else if (expectedStatusCode == 500)
            {
                var serverError = Assert.IsType<ObjectResult>(result);
                Assert.Equal(expectedStatusCode, serverError.StatusCode);
                Assert.Contains(expectedMessage, serverError.Value.ToString());
            }
            else
            {
                var okResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(expectedStatusCode, okResult.StatusCode);
                Assert.Contains(expectedMessage, okResult.Value.ToString());
            }
        }


        [Theory]
        [InlineData("/ru", 200)]
        [InlineData("", 400)]
        [InlineData("---", 500)]
        public void FindPlatforms_list(string location, int statusCode)
        {
            var mokLogger = new Mock<ILogger<PlatformService>>();
            PlatformService service = new PlatformService(mokLogger.Object);
            service.LocationAllPlatforms = new Dictionary<string, List<string>>
            {
                { "ru", new List<string>{ "Яндекс.Директ", "Крутая реклама" } },
                { "svrd", new List<string>{ "Крутая реклама" } },
            };

            var mokControllerLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(mokControllerLogger.Object, service);

            var result = controller.FindPlatforms(location);
            if(statusCode == 200)
            {
                var response = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(statusCode, response.StatusCode);
            }
            else if(statusCode == 400)
            {
                var response = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal(statusCode, response.StatusCode);
            }
            else
            {
                var response = Assert.IsType<ObjectResult>(result);
                Assert.Equal(statusCode, response.StatusCode);
            }

        }

    }
}