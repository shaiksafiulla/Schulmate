using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using System.Diagnostics;

namespace SchoolCoreAPI.Services
{
    //public class DatabaseBackupService : IDatabaseBackupService
    //{

    //    private readonly IWebHostEnvironment _hostingEnvironment;
    //    private readonly string _backupDirectory;      
    //    private readonly string _pgDumpPath;
    //    private readonly string _connectionDetails;
    //    private readonly string _databaseName;
    //    private readonly string _user;
    //    private readonly string _password;
    //    private readonly string _host;

    //    public DatabaseBackupService(IWebHostEnvironment hostingEnvironment)
    //    {
    //        _hostingEnvironment = hostingEnvironment;          
    //        _backupDirectory = Path.Combine(_hostingEnvironment.WebRootPath, "Backups");           
    //        _pgDumpPath = @"C:\Program Files\PostgreSQL\17\bin\pg_dump.exe";      
    //        _databaseName = "schooldb";  // Modify as needed
    //        _user = "postgres";  // Modify as needed
    //        _password = "P@roject22a";  // Modify as needed
    //        _host = "localhost";  // Modify as needed
    //    }
    //    public void PerformBackup()
    //    {
    //        try
    //        {               
    //            if (!Directory.Exists(_backupDirectory))
    //            {
    //                Directory.CreateDirectory(_backupDirectory);
    //            }              
    //            string backupFileName = $"DBBackup_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
    //            string backupFilePath = Path.Combine(_backupDirectory, backupFileName);               
    //            Environment.SetEnvironmentVariable("PGPASSWORD", _password);
    //            string arguments = $"-F c -b -v -U {_user} -h {_host} -d {_databaseName} -f \"{backupFilePath}\"";           
    //            var processStartInfo = new ProcessStartInfo
    //            {
    //                FileName = _pgDumpPath,
    //                Arguments = arguments,
    //                RedirectStandardError = true,
    //                UseShellExecute = false,
    //                CreateNoWindow = true
    //            };

    //            using (var process = new Process { StartInfo = processStartInfo })
    //            {
    //                process.Start();
    //                string error = process.StandardError.ReadToEnd();
    //                process.WaitForExit();

    //                if (process.ExitCode == 0)
    //                {
    //                    Console.WriteLine($"Database backup completed successfully: {backupFilePath}");
    //                }
    //                else
    //                {
    //                    throw new Exception($"mysqldump failed: {error}");
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Database backup failed: {ex.Message}");
    //        }
    //    }
    //}
    public class DatabaseBackupService : IDatabaseBackupService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly string _backupDirectory;
        private readonly string _pgDumpCommand = "pg_dump";

        private readonly string _host;
        private readonly string _database;
        private readonly string _username;
        private readonly string _password;

        public DatabaseBackupService(IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;

            _backupDirectory = Path.Combine(_hostingEnvironment.WebRootPath ?? "/app/wwwroot", "Backups");
            Directory.CreateDirectory(_backupDirectory);

            var connString = configuration.GetConnectionString("DefaultConnection");
            var connParts = connString.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in connParts)
            {
                var keyValue = part.Split('=', 2); // Split only at the first '='
                if (keyValue.Length != 2) continue;

                var key = keyValue[0].Trim();
                var value = keyValue[1].Trim();

                switch (key.ToLowerInvariant())
                {
                    case "host":
                        _host = value;
                        break;
                    case "database":
                        _database = value;
                        break;
                    case "username":
                        _username = value;
                        break;
                    case "password":
                        _password = value;
                        break;
                }
            }
            // Simple parser for PostgreSQL connection string
            //var connParts = connString.Split(';');
            //foreach (var part in connParts)
            //{
            //    if (part.StartsWith("Host=", StringComparison.OrdinalIgnoreCase))
            //        _host = part.Split('=')[1];
            //    else if (part.StartsWith("Database=", StringComparison.OrdinalIgnoreCase))
            //        _database = part.Split('=')[1];
            //    else if (part.StartsWith("Username=", StringComparison.OrdinalIgnoreCase))
            //        _username = part.Split('=')[1];
            //    else if (part.StartsWith("Password=", StringComparison.OrdinalIgnoreCase))
            //        _password = part.Split('=')[1];
            //}

            //if (string.IsNullOrWhiteSpace(_host) || string.IsNullOrWhiteSpace(_database) ||
            //    string.IsNullOrWhiteSpace(_username) || string.IsNullOrWhiteSpace(_password))
            //{
            //    throw new InvalidOperationException("PostgreSQL connection string is missing required components.");
            //}
        }

        public void PerformBackup()
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupFileName = $"DBBackup_{timestamp}.sql";
                string backupFilePath = Path.Combine(_backupDirectory, backupFileName);

                Environment.SetEnvironmentVariable("PGPASSWORD", _password);

                string arguments = $"-F c -b -v -U {_username} -h {_host} -d {_database} -f \"{backupFilePath}\"";

                var startInfo = new ProcessStartInfo
                {
                    FileName = _pgDumpCommand,
                    Arguments = arguments,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = startInfo };
                process.Start();

                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Console.WriteLine($"✅ Database backup completed successfully: {backupFilePath}");
                }
                else
                {
                    throw new Exception($"❌ pg_dump failed: {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database backup failed: {ex.Message}");
            }
        }
    }
}
