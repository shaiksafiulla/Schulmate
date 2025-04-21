namespace SchoolCoreAPI.Helpers
{
    public class HostEnvironmentService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public HostEnvironmentService(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public string GetContentRootPath()
        {
            return _hostingEnvironment.ContentRootPath;
        }
        public string GetWebRootPath()
        {
            return _hostingEnvironment.WebRootPath;
        }
        public string GetFullUrlFromDbPath(string dbpath)
        {
            return Path.Combine(_hostingEnvironment.WebRootPath, dbpath);
        }
        public string GetUploadPath()
        {
            return Path.Combine(_hostingEnvironment.WebRootPath, "Uploads");            
        }
    }
}
