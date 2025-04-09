
using KrutayaReklama.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;

namespace KrutayaReklama.Controllers
{
    public class HomeController : Controller
    {

        private readonly DataStoregeService _storage;
        

        public HomeController(DataStoregeService dataStoregeService)
        {
           _storage = dataStoregeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UploadFile([FromForm] IFormFile file)
        {
            using (StreamReader reader = new StreamReader(file.OpenReadStream()))
            {
                while (!reader.EndOfStream)
                {
                    try
                    {
                        var platformString = reader.ReadLine().Split(":");
                        string platformName = platformString[0];
                        HashSet<Location> platformLocations = new();

                        var locationsGroups = platformString[1].Split(",");

                        foreach (var locationGroup in locationsGroups)
                        {
                            Location last = null;
                            {
                                var locations = locationGroup.Split('/');
                                Location parent = null;

                                for (int i = 1; i < locations.Length; i++)
                                {
                                    Location location = new Location();
                                    location.Title = locations[i];
                                    location.Parent = parent;

                                    if (!_storage.locations.Add(location))
                                        _storage.locations.TryGetValue(location, out parent);

                                    parent = location;
                                    last = location;
                                }
                            }

                            platformLocations.Add(last);
                        }
                        try
                        {
                            _storage.platforms.Add(platformName, platformLocations);
                        }
                        catch{ }
                    }
                    catch (Exception ex) {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine($"current platforms count: [{_storage.platforms.Count}]");
                        Console.WriteLine($"current locations count: [{_storage.locations.Count}]");
                        return Json(new { exception = ex.GetType().Name, message = ex.Message });
                    };
                }

            }

            Console.WriteLine($"all Locations [{_storage.locations.Count}]:\n");
            foreach (var p in _storage.locations)
                Console.Write("[" + p.Title + "]");
            Console.WriteLine(" \n\n");

            Console.WriteLine($"all platforms [{_storage.platforms.Count}]:\n");
            foreach (var p in _storage.platforms)
                Console.Write("[" + p.Key + "]");
            Console.WriteLine(" \n\n");

            return Json(new { message = "sucseed" });
        }




        public IActionResult FindPlatforms(string location)
        {
            Console.WriteLine("searching here: ");
            List<string> platforms = new();

            var locations = location.Split('/');
            Console.WriteLine(locations[locations.Length - 1]);
            Location location1 = _storage.locations.FirstOrDefault(l => l.Title == locations[locations.Length-1]);
            if (location1 == null)
                return Json(new { message = "no platforms found" });

            FindInParent(platforms, location1);

            Console.WriteLine($"found {platforms.Count} platforms: ");
            foreach (var p in platforms)
                Console.WriteLine(p);   

            Console.WriteLine();
            return Json(platforms);
        }

        public void FindInParent(List<string> platforms, Location location)
        {         
            foreach (var platform in _storage.platforms)
                if (platform.Value.Contains(location))
                    platforms.Add(platform.Key);

            if (location.Parent != null)
                FindInParent(platforms, location.Parent);

            else return;
        }
    }

    public class Location
    {
        public string Title { get; set; } = string.Empty;
        public Location Parent { get; set; } = null;
        public override bool Equals(object obj)
        {
            if (obj is not Location other) return false;
            return Title == other.Title;
        }

        public override int GetHashCode()
        {
            return Title.GetHashCode();
        }
    }
}
