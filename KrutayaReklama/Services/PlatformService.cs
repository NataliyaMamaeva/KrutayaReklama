
using KrutayaReklama.Controllers;

namespace KrutayaReklama.Services

{
    public class PlatformService : IPlatformService
    {
        
        public Dictionary<string, List<string>> LocationAllPlatforms = new Dictionary<string, List<string>>();
        private readonly ILogger<PlatformService> _logger;

        public PlatformService(ILogger<PlatformService> logger)
        {
            _logger = logger;
        }
        public List<string> FindPlatforms(string location)
        {
            if (!LocationAllPlatforms.TryGetValue(location, out var list))
                throw new ArgumentException("локация не найдена");  

            return list;        
        }

        public void UploadFile(IFormFile file)
        {
            using (StreamReader reader = new StreamReader(file.OpenReadStream()))
            {

             
                while (!reader.EndOfStream)
                {
                    var platformString = reader.ReadLine().Split(":");

                    string platformName = platformString[0];
                    var locationsGroups = platformString[1].Split(",");

                    foreach (var locationGroup in locationsGroups)
                    {
                        foreach (var location in locationGroup.Split("/"))
                        {
                         
                            if(LocationAllPlatforms.TryGetValue(location, out var list))
                            {
                                if (!list.Contains(platformName))
                                    list.Add(platformName);
                            }                             
                            else
                            {
                                LocationAllPlatforms.Add(location, new List<string> { platformName });
                            }                 
                        }
                    }
                }
            }
            _logger.LogInformation("File has been uploaded. Current locations count: {Count}", LocationAllPlatforms.Count);
        }
    }
}
