using Moq;
using KrutayaReklama;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using KrutayaReklama.Services;
using KrutayaReklama.Controllers;
using Microsoft.AspNetCore.Http;


namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public void UploadFile_ok()
        {
            var mok = new Mock<IFormFile>();
            var content = "яндекс.ƒирект:/ru\r\n" +
                "–евдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik\r\n" +
                "√азета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl\r\n" +
                " рута€ реклама:/ru/svrd\r\n";

            var fileStream = new MemoryStream();
            var writer = new StreamWriter(fileStream);
            writer.Write(content);
            writer.Flush();
            fileStream.Position = 0;
            mok.Setup(f => f.OpenReadStream()).Returns(fileStream);
            DataStoregeService service = new DataStoregeService();
            
            var controller = new HomeController(service);
            var result = controller.UploadFile(mok.Object);
            var json = Assert.IsType<JsonResult>(result);
            //var message = Assert.IsType<string>(json.Value);
            Assert.Equal("{ message = sucseed }", json.Value.ToString());
        }


        [Fact]
        public void FindPlatforms_list()
        {
            DataStoregeService service = new DataStoregeService();
            Location l1 = new Location() { Title = "ru", Parent = null };
            Location l2 = new Location() { Title = "chelobl", Parent = l1 };
            Location l3 = new Location() { Title = "varna", Parent = l2 };
            Location l4 = new Location() { Title = "pokrovka", Parent = l3 };
            Location l5 = new Location() { Title = "svrd", Parent = l1 };
            Location l6 = new Location() { Title = "ekb", Parent = l5 };

            service.platforms = new Dictionary<string, HashSet<Location>> { { "уральский труд€га", new() {l4} },
                { "€ндексƒирект", new() { l1 } },{"мегаполис", new() { l6} } };

            service.locations = new() { l1, l2, l3, l4, l5, l6 };

            var controller = new HomeController(service);

            var result = controller.FindPlatforms("pokrovka");
            List<string> expected = new List<string>() {  "уральский труд€га", "€ндексƒирект" };

            var isJson = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsType<List<string>>(isJson.Value);

            Assert.Equal(expected, actual);
        }

    }
}