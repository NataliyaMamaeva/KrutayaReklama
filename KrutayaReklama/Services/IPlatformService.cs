namespace KrutayaReklama.Services
{
    public interface IPlatformService
    {
        void UploadFile(IFormFile file);
        List<string> FindPlatforms (string location);
    }
}
