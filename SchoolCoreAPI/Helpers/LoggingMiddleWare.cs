using Newtonsoft.Json;
using SchoolCoreAPI.Models;
using System.Data;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace SchoolCoreAPI.Helpers
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly string _logFilePath;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, IWebHostEnvironment hostingEnvironment)
        {
            _next = next;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            // Set the log file path relative to the application's root directory
            // _logFilePath = Path.Combine(_hostingEnvironment.GetWebRootPath(), "logs", "requests.log");           
            _logFilePath = Path.Combine(_hostingEnvironment.WebRootPath ?? "/app/wwwroot", "logs/requests.log");
            Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath)); // Ensure the logs directory exists
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // Log request details
            _logger.LogInformation("Handling request: {Method} {Url}", context.Request.Method, context.Request.Path);

            await _next(context); // Call the next middleware in the pipeline

            stopwatch.Stop();

            // Log response details
            _logger.LogInformation($"Outgoing response: {context.Response.StatusCode} in {stopwatch.ElapsedMilliseconds} ms");

            var responseTime = stopwatch.ElapsedMilliseconds;

            // Log the response details
            _logger.LogInformation("Finished handling request. Status Code: {StatusCode} Response Time: {ResponseTime} ms", context.Response.StatusCode, responseTime);

            // Write to the log file
            LogToFile(context.Request.Method, context.Request.Path, context.Response.StatusCode, responseTime);
        }
        //private void LogToFile(string method, string url, int statusCode, long responseTime)
        //{           

        //    // Rotate logs if they exceed 1 MB
        //    var logFileInfo = new FileInfo(_logFilePath);
        //    if (logFileInfo.Exists && logFileInfo.Length > 1 * 1024 * 1024) // 1 MB
        //    {
        //        string newLogFileName = Path.Combine(Path.GetDirectoryName(_logFilePath), $"requests_{DateTime.Now:yyyyMMdd_HHmmss}.log");
        //        File.Move(_logFilePath, newLogFileName);
        //    }

        //    // Log the new entry
        //    var localTime = DateTime.Now;
        //    var logMessage = $"{localTime:yyyy-MM-dd HH:mm:ss} - {method} {url} - Status Code: {statusCode} - Response Time: {responseTime} ms\n";
        //    File.AppendAllText(_logFilePath, logMessage);
        //}
        private static readonly object _logLock = new object();
        private void LogToFile(string method, string url, int statusCode, long responseTime)
        {
            lock (_logLock)
            {
                // Rotate logs if they exceed 1 MB
                var logFileInfo = new FileInfo(_logFilePath);
                if (logFileInfo.Exists && logFileInfo.Length > 1 * 1024 * 1024) // 1 MB
                {
                    string newLogFileName = Path.Combine(Path.GetDirectoryName(_logFilePath), $"requests_{DateTime.Now:yyyyMMdd_HHmmss}.log");
                    File.Move(_logFilePath, newLogFileName);
                }

                // Log the new entry
                var localTime = DateTime.Now;
                var logMessage = $"{localTime:yyyy-MM-dd HH:mm:ss} - {method} {url} - Status Code: {statusCode} - Response Time: {responseTime} ms\n";
                File.AppendAllText(_logFilePath, logMessage);
            }
        }
    }


}