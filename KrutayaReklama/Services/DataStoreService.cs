using KrutayaReklama.Controllers;

namespace KrutayaReklama.Services
{
    public class DataStoregeService
    {
        public  Dictionary<string, HashSet<Location>> platforms { get; set; } = new();
        public  HashSet<Location> locations { get; set; } = new HashSet<Location>();
      

    }
}
