using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Helpers
{
    public class S3Settings
    {
        public string ServiceURL { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }
        public string Region { get; set; }
    }

    public class S3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly S3Settings _settings;

        public S3Service(IOptions<S3Settings> settings)
        {
            _settings = settings.Value;

            var config = new AmazonS3Config
            {
                ServiceURL = _settings.ServiceURL,
                ForcePathStyle = true, // important for MinIO
               // RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_settings.Region)
            };

            _s3Client = new AmazonS3Client(_settings.AccessKey, _settings.SecretKey, config);
        }

        //public async Task<string> UploadFileAsync(IFormFile file, string folderType)
        //{
        //    var fileName = $"{folderType}/{Guid.NewGuid()}_{file.FileName}";

        //    using var newMemoryStream = new MemoryStream();
        //    await file.CopyToAsync(newMemoryStream);

        //    var uploadRequest = new TransferUtilityUploadRequest
        //    {
        //        InputStream = newMemoryStream,
        //        Key = fileName,
        //        BucketName = _settings.BucketName,
        //        ContentType = file.ContentType
        //    };

        //    var transferUtility = new TransferUtility(_s3Client);
        //    await transferUtility.UploadAsync(uploadRequest);

        //    return fileName;
        //}
        private async Task EnsureBucketExistsAsync()
        {
            var exists = await _s3Client.DoesS3BucketExistAsync(_settings.BucketName);
            if (!exists)
            {
                await _s3Client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = _settings.BucketName,
                    UseClientRegion = true
                });
            }
        }      
        public async Task<string> UploadFileAsync(int type, IFormFile uploadFile)
        {
            if (uploadFile == null || uploadFile.Length == 0)
                return null;

            string uploadFileType = GetUploadFileType(type);
            if (string.IsNullOrEmpty(uploadFileType))
                return null;

            try
            {
                // Ensure bucket exists before uploading
                await EnsureBucketExistsAsync();

                string uniqueFileName = $"{Guid.NewGuid()}_{uploadFile.FileName}";
                string s3Key = $"{uploadFileType}/{uniqueFileName}";

                using var newMemoryStream = new MemoryStream();
                await uploadFile.CopyToAsync(newMemoryStream);

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = newMemoryStream,
                    Key = s3Key,
                    BucketName = _settings.BucketName,
                    ContentType = uploadFile.ContentType
                };

                var transferUtility = new TransferUtility(_s3Client);
                await transferUtility.UploadAsync(uploadRequest);

                return "/" + _settings.BucketName +"/" + s3Key; // return the S3 path for database or frontend use
            }
            catch (Exception ex)
            {
                // Log the error if needed
                Console.WriteLine($"Upload failed: {ex.Message}");
                return null;
            }
        }


        private string GetUploadFileType(int type)
        {
            return type switch
            {
                (int)UploadType.Student => "Student",
                (int)UploadType.Teacher => "Teacher",
                (int)UploadType.QuestionPaper => "QuestionPaper",
                (int)UploadType.Organization => "Organization",
                (int)UploadType.ReportSettings => "ReportSettings",
                (int)UploadType.Attachment => "Attachment",
                (int)UploadType.Assignment => "Assignment",
                (int)UploadType.Admin => "Admin",
                (int)UploadType.Staff => "Staff",
                (int)UploadType.Driver => "Driver",
                (int)UploadType.Announcement => "Announcement",
                (int)UploadType.Notification => "Notification",
                (int)UploadType.Chatbot => "Chatbot",
                _ => null
            };
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _settings.BucketName,
                    Key = filePath
                };

                var response = await _s3Client.DeleteObjectAsync(deleteRequest);
                return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
            }
            catch (AmazonS3Exception ex)
            {
                // Optional: log or handle
                return false;
            }
        }

        public async Task<byte[]> GetS3ObjectAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            string prefixToTrim = "/uploads/";
            //string key = filePath.TrimStart('/uploads/');
            string key = filePath.StartsWith(prefixToTrim)
    ? filePath.Substring(prefixToTrim.Length)
    : filePath;

            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = _settings.BucketName,
                    Key = key
                };

                using var response = await _s3Client.GetObjectAsync(request);
                using var memoryStream = new MemoryStream();
                await response.ResponseStream.CopyToAsync(memoryStream);

                var fileBytes = memoryStream.ToArray();
               // string contentType = response.Headers.ContentType;

                return fileBytes;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"S3 GetObject failed: {ex.Message}");
                return null;
            }
        }


    }
}
