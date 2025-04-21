using Newtonsoft.Json;
using SchoolCoreAPI.Models;
using System.Data;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using WebPush;

namespace SchoolCoreAPI.Helpers
{
    public class Shared
    {
        public enum UserType
        {
            SuperAdmin = 1, Admin = 2, Teacher = 3, Student = 4, Staff = 5, Driver = 6, EveryOne = 7
        }
        public enum TagType
        {
            FeeReminder = 1, AbsentStudents = 2
        }
        public enum LeaveRequestType
        {
            Casual = 1, Sick = 2
        }
        public enum TransferType
        {
            Admitted = 1, Transfer = 2, Promoted = 3, Leave = 4
        }
        public enum PercentFixedType
        {
            Percentage = 1, Fixed = 2
        }
        public enum AssignmentStatus
        {
            New = 0, Accepted = 1, Rejected = 2, 
        }
        public enum LeaveRequestStatus
        {
            UnderProcess = 1, Rejected = 2, Approved = 3
        }
        public enum SubjectType
        {
            Theory = 1, Practical = 2
        }
        public enum QPaperType
        {
            Upload = 1,
            Paper = 2,
            AutoChoose = 3
        }
        public enum FeedType
        {
            Excel = 1,
            Manual = 2
            
        }
        public enum ActiveState
        {
            Active = 1, InActive = 0, ActiveAdmin = 2,
        }
        public enum ReadStatus
        {
            UnRead = 1, Read = 0
        }
        public enum NotificationType
        {
            LeaveRequest = 1, StudentAssignment= 2, TeacherAnnouncement =3, AdminAnnouncement =4, AdminNotification = 5
        }
        public enum Gender
        {
            Male = 1, Female = 2,
        }
        public enum EventType
        {
            Event = 1, Holiday = 2,
        }
        public enum LoginStatus
        {
            Valid = 1, InValid = 0
        }
        public enum LicenseStatus
        {
            Valid = 1, InValid = 0
        }
        public enum ServiceResultStatus
        {
            Added = 1, Updated = 2, Deleted = 3
        }
        public enum UploadType
        {
            Student = 1, Teacher = 2, QuestionPaper = 3, Organization = 4, ReportSettings = 5, Attachment = 6, Assignment = 7, Admin = 8, Staff = 9, Driver = 10, Announcement = 11, Notification = 12, Chatbot = 13
        }
        public enum ScheduleStatus
        {
            Schedule = 1,
            Subject = 2,
            TimeTable = 3,
            Student = 4,
            Teacher = 5,            
            Finish = 6,
            Syllabus = 7,
            Result = 8
        }
        public enum AttendanceStatus
        {
            Present =1,
            Absent = 0,
            Leave =2
        }
        public (string PublicKey, string PrivateKey) GenerateVapidKeys()
        {
            var vapidKeys = VapidHelper.GenerateVapidKeys();
            return (vapidKeys.PublicKey, vapidKeys.PrivateKey);
        }
        public static T GetUserParamFromHeader<T>(IHeaderDictionary headers,string headerName)
        {
            var userParamHeader = headers[headerName].ToString();
            if (string.IsNullOrEmpty(userParamHeader))
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(userParamHeader);
        }
        public static string ToSHA2569(string value)
        {
            System.Security.Cryptography.SHA256 sha256 = SHA256.Create();
            byte[] hashData = sha256.ComputeHash(Encoding.Default.GetBytes(value));
            StringBuilder returnValue = new StringBuilder();

            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }

            return returnValue.ToString();
        }
        public static class ListtoDataTableConverter
        {
            public static DataTable ToDataTable<T>(List<T> items)
            {
                DataTable dataTable = new DataTable(typeof(T).Name);
                //Get all the properties
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    //Setting column names as Property names
                    dataTable.Columns.Add(prop.Name);
                }
                foreach (T item in items)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                        //inserting property values to datatable rows
                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }
                //put a breakpoint here and check datatable
                return dataTable;
            }
        }
        public static DataTable ConvertToDataTable(IEnumerable<dynamic> pivotedData)
        {
            // Create a new DataTable
            var dataTable = new DataTable();

            // Check if there is any data
            if (!pivotedData.Any())
                return dataTable;

            // Get the first row of data to create the columns
            var firstRow = pivotedData.First();

            // Add columns for each key (ClassName, BranchClassId, and Subjects)
            foreach (var property in (IDictionary<string, object>)firstRow)
            {
                dataTable.Columns.Add(property.Key);
            }

            // Add rows for each item in the pivoted data
            foreach (var row in pivotedData)
            {
                var dataRow = dataTable.NewRow();
                foreach (var property in (IDictionary<string, object>)row)
                {
                    dataRow[property.Key] = property.Value ?? DBNull.Value; // Handle null values
                }
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }
        public static DataTable ConvertListToDataTable(List<Fn_RccP> studentResults)
        {
            DataTable dt = new DataTable();

            // Check if the list is not empty to create the columns
            if (studentResults.Any())
            {
                // Get the properties of Fn_RccP to create columns dynamically
                var properties = typeof(Fn_RccP).GetProperties();

                // Add columns for each property in the Fn_RccP class
                foreach (var property in properties)
                {
                    dt.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }

                // Add rows to the DataTable from the list of Fn_RccP
                foreach (var studentResult in studentResults)
                {
                    DataRow row = dt.NewRow();

                    // Fill the row with values from the studentResult object
                    foreach (var property in properties)
                    {
                        row[property.Name] = property.GetValue(studentResult) ?? DBNull.Value;
                    }

                    dt.Rows.Add(row);
                }
            }

            return dt;
        }
        public static string GetNoImagePath()
        {
          // return System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/NoImage/noimage.png");
            return "/uploads/NoImage/noimage.png";
        }
        public static string GetNoUserPath()
        {
            return "/uploads/NoImage/nouser.png";
        }
       

        //public static string ProcessUploadFile(int type, IFormFile uploadfile, string rootpath)
        //{
        //    string fullPath = null; string uploadFileType = null;
        //    try
        //    {
        //        if (uploadfile != null)
        //        {
        //            switch (type)
        //            {
        //                case (int)UploadType.Student:
        //                    uploadFileType = "Student";
        //                    break;
        //                case (int)UploadType.Teacher:
        //                    uploadFileType = "Teacher";
        //                    break;
        //                case (int)UploadType.QuestionPaper:
        //                    uploadFileType = "QuestionPaper";
        //                    break;
        //                case (int)UploadType.Organization:
        //                    uploadFileType = "Organization";
        //                    break;
        //                case (int)UploadType.ReportSettings:
        //                    uploadFileType = "ReportSettings";
        //                    break;
        //                case (int)UploadType.Attachment:
        //                    uploadFileType = "Attachment";
        //                    break;
        //                case (int)UploadType.Assignment:
        //                    uploadFileType = "Assignment";
        //                    break;
        //                case (int)UploadType.Admin:
        //                    uploadFileType = "Admin";
        //                    break;
        //                case (int)UploadType.Staff:
        //                    uploadFileType = "Staff";
        //                    break;
        //                case (int)UploadType.Driver:
        //                    uploadFileType = "Driver";
        //                    break;
        //                case (int)UploadType.Announcement:
        //                    uploadFileType = "Announcement";
        //                    break;
        //                case (int)UploadType.Notification:
        //                    uploadFileType = "Notification";
        //                    break;
        //                case (int)UploadType.Chatbot:
        //                    uploadFileType = "Chatbot";
        //                    break;
        //            }
        //            string Folderpath = Path.Combine(rootpath, "Uploads", uploadFileType);
        //            if (!Directory.Exists(Folderpath))
        //            {
        //                Directory.CreateDirectory(Folderpath);
        //            }
        //            string uniqueFileName = Guid.NewGuid().ToString() + "_" + uploadfile.FileName;
        //            string filePath = Path.Combine(Folderpath, uniqueFileName);
        //            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        //            {
        //                //await Task.Delay(10000);
        //                uploadfile.CopyTo(fileStream);
        //                //fileStream.FlushAsync();
        //            }
        //            fullPath = Path.Combine("Uploads", uploadFileType, uniqueFileName);
        //        }               
        //    }
        //    catch (Exception ex)
        //    {               
        //    }
        //    return fullPath;
        //}     
       
        public static string DataTableToJSON(DataTable table)
        {
            string JSONString = string.Empty;
            //JsonSerializerSettings settings = new JsonSerializerSettings
            //{
            //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            //};
            JSONString = JsonConvert.SerializeObject(table, Formatting.Indented);            
            return JSONString;
        }
        public static TimeSpan GetTimeSpan(string strTime)
        {
            return TimeSpan.Parse(strTime);
        }
        public static CastTimetable GenerateTimetable(int periodDurationMinutes, List<Break> breaks, TimeSpan schoolStart, TimeSpan schoolEnd)
        {
            var timetable = new CastTimetable
            {
                Weekdays = new List<Weekday>(),
                Periods = new List<Period>()
            };

            // Generate periods
            List<Period> periods = new List<Period>();
            TimeSpan currentTime = schoolStart;

            while (currentTime < schoolEnd)
            {
                // Check if current time is a break
                var currentBreak = breaks.FirstOrDefault(b => currentTime >= b.StartTime && currentTime < b.EndTime);
                if (currentBreak != null)
                {
                    periods.Add(new Period
                    {
                        StartTime = currentBreak.StartTime,
                        EndTime = currentBreak.EndTime,
                        IsBreak = true,
                        Description = currentBreak.Name
                    });
                    currentTime = currentBreak.EndTime;
                }
                else
                {
                    // Regular period
                    TimeSpan periodEnd = currentTime.Add(TimeSpan.FromMinutes(periodDurationMinutes));

                    // Ensure not to overlap with a break
                    var nextBreak = breaks.FirstOrDefault(b => b.StartTime > currentTime);
                    if (nextBreak != null && periodEnd > nextBreak.StartTime)
                    {
                        periodEnd = nextBreak.StartTime;
                    }

                    periods.Add(new Period
                    {
                        StartTime = currentTime,
                        EndTime = periodEnd,
                        IsBreak = false,
                        Description = $"Period {periods.Count + 1}"
                    });
                    currentTime = periodEnd;
                }
            }

            timetable.Periods = periods;

            // Initialize weekdays
            string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            foreach (var day in days)
            {
                timetable.Weekdays.Add(new Weekday
                {
                    Name = day,
                    PeriodSlots = periods.Select(p => new PeriodSlot
                    {
                        Period = p,
                        Subject = p.IsBreak ? p.Description : "" // Placeholder
                    }).ToList()
                });
            }

            return timetable;
        }
        public static CastTransTimetable GenerateTransTimetable(int periodDurationMinutes, List<Break> breaks, TimeSpan schoolStart, TimeSpan schoolEnd)
        {
            var timetable = new CastTransTimetable
            {
                Weekdays = new List<WeekdayColumn>(),
                Periods = new List<Period>()
            };

            // Generate periods
            List<Period> periods = new List<Period>();
            TimeSpan currentTime = schoolStart;

            while (currentTime < schoolEnd)
            {
                // Check if current time is a break
                var currentBreak = breaks.FirstOrDefault(b => currentTime >= b.StartTime && currentTime < b.EndTime);
                if (currentBreak != null)
                {
                    periods.Add(new Period
                    {
                        StartTime = currentBreak.StartTime,
                        EndTime = currentBreak.EndTime,
                        IsBreak = true,
                        Description = currentBreak.Name
                    });
                    currentTime = currentBreak.EndTime;
                }
                else
                {
                    // Regular period
                    TimeSpan periodEnd = currentTime.Add(TimeSpan.FromMinutes(periodDurationMinutes));

                    // Ensure not to overlap with a break
                    var nextBreak = breaks.FirstOrDefault(b => b.StartTime > currentTime);
                    if (nextBreak != null && periodEnd > nextBreak.StartTime)
                    {
                        periodEnd = nextBreak.StartTime;
                    }

                    periods.Add(new Period
                    {
                        StartTime = currentTime,
                        EndTime = periodEnd,
                        IsBreak = false,
                        Description = $"Period {periods.Count + 1}"
                    });
                    currentTime = periodEnd;
                }
            }

            timetable.Periods = periods;

            // Initialize weekdays columns
            string[] days = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

            foreach (var day in days)
            {
                timetable.Weekdays.Add(new WeekdayColumn
                {
                    Name = day,
                    Subjects = periods.Select(p => p.IsBreak ? p.Description : "").ToList()  // Default empty subjects (could be populated later)
                });
            }

            return timetable;
        }
        private static byte[] Compress(byte[] data)
        {
            using (MemoryStream compressedStream = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Compress, leaveOpen: true))
                {
                    gzipStream.Write(data, 0, data.Length);
                }

                return compressedStream.ToArray();
            }
        }
        //static string passcode = string.Empty;
        //public static string SendEmail(PersonEmail obj)
        //{
        //    try
        //    {
        //        EmailBody emailbody = PopulateBody(obj);
        //        // SmtpServer oServer = new SmtpServer("");
        //        SmtpSection smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
        //        using (MailMessage mm = new MailMessage(smtpSection.From, obj.ToAddress))
        //        {
        //            mm.SubjectEncoding = System.Text.Encoding.UTF8;
        //            mm.BodyEncoding = System.Text.Encoding.UTF8;

        //            mm.Subject = emailbody.Subject;
        //            mm.Body = emailbody.Body;
        //            mm.IsBodyHtml = true;

        //            SmtpClient smtp = new SmtpClient();
        //            smtp.Host = smtpSection.Network.Host;
        //            smtp.EnableSsl = smtpSection.Network.EnableSsl;
        //            NetworkCredential networkCred = new NetworkCredential(smtpSection.Network.UserName, smtpSection.Network.Password);
        //            smtp.UseDefaultCredentials = smtpSection.Network.DefaultCredentials;
        //            smtp.Credentials = networkCred;
        //            smtp.DeliveryMethod = smtpSection.DeliveryMethod;
        //            smtp.Port = smtpSection.Network.Port;
        //            smtp.Send(mm);

        //        }
        //        return emailbody.Passcode;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        //public static int GenerateRandomNo()
        //{
        //    int _min = 1000;
        //    int _max = 9999;
        //    Random _rdm = new Random();
        //    return _rdm.Next(_min, _max);
        //}


        //public static EmailBody PopulateBody(PersonEmail obj)
        //{
        //    try
        //    {
        //        EmailBody emailBody = new EmailBody();
        //        // string ImgPath = HttpContext.Current.Server.MapPath(@"~/Images/SchoolAPI_logo.png");
        //        string ImgPath = Path.Combine(HttpRuntime.AppDomainAppPath, "Images/SchoolAPI_logo.png");
        //        string body = string.Empty; string title = string.Empty; string description = string.Empty; string subject = string.Empty;
        //        using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath(@"~/EmailTemplate.html")))
        //        {
        //            body = reader.ReadToEnd();
        //        }
        //        switch (obj.MessageType)
        //        {
        //            case (int)EmailMessageType.Registration:
        //                subject = "Registration Successful !";
        //                title = "Your Account No is " + obj.AccountNo;
        //                description = "Please proceed to the Payments and activate your account";
        //                break;
        //            case (int)EmailMessageType.Payment:
        //                subject = "Your Payment has been received !";
        //                title = "Your Payment Reference No is " + obj.PayReferenceNo;
        //                description = "Your Account will be activated soon";
        //                break;
        //            case (int)EmailMessageType.Activation:
        //                subject = "Your Account is Activated...";
        //                title = "Congrats ! Your Account is Now Online...";
        //                description = "Your can manage your account efficiently..Please Login to use the services !";
        //                break;
        //            case (int)EmailMessageType.LoginOTP:
        //                passcode = GenerateRandomNo().ToString();
        //                subject = "Your Login Password is :";
        //                title = passcode;
        //                description = "Please don't share to anyone";
        //                break;
        //        }

        //        body = body.Replace("{UserName}", obj.UserName);
        //        body = body.Replace("{Title}", title);
        //        body = body.Replace("{Description}", description);
        //        body = body.Replace("{ImgPath}", ImgPath);

        //        emailBody.Body = body;
        //        emailBody.Subject = subject;
        //        emailBody.Passcode = passcode;

        //        return emailBody;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}

        //public static string SendOwnerEmail(PersonEmail obj)
        //{
        //    try
        //    {
        //        EmailBody emailbody = PopulateOwnerBody(obj);
        //        // SmtpServer oServer = new SmtpServer("");
        //        SmtpSection smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
        //        using (MailMessage mm = new MailMessage(smtpSection.From, obj.ToAddress))
        //        {
        //            mm.SubjectEncoding = System.Text.Encoding.UTF8;
        //            mm.BodyEncoding = System.Text.Encoding.UTF8;

        //            mm.Subject = emailbody.Subject;
        //            mm.Body = emailbody.Body;
        //            mm.IsBodyHtml = true;

        //            SmtpClient smtp = new SmtpClient();
        //            smtp.Host = smtpSection.Network.Host;
        //            smtp.EnableSsl = smtpSection.Network.EnableSsl;
        //            NetworkCredential networkCred = new NetworkCredential(smtpSection.Network.UserName, smtpSection.Network.Password);
        //            smtp.UseDefaultCredentials = smtpSection.Network.DefaultCredentials;
        //            smtp.Credentials = networkCred;
        //            smtp.DeliveryMethod = smtpSection.DeliveryMethod;
        //            smtp.Port = smtpSection.Network.Port;
        //            smtp.Send(mm);
        //        }
        //        return emailbody.Passcode;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        //public static EmailBody PopulateOwnerBody(PersonEmail obj)
        //{
        //    try
        //    {
        //        EmailBody emailBody = new EmailBody();
        //        //  string ImgPath = HttpContext.Current.Server.MapPath(@"~/Images/SchoolAPI_logo.png");
        //        string ImgPath = Path.Combine(HttpRuntime.AppDomainAppPath, "Images/SchoolAPI_logo.png");
        //        string body = string.Empty; string title = string.Empty; string description = string.Empty; string subject = string.Empty;
        //        using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath(@"~/EmailTemplate.html")))
        //        {
        //            body = reader.ReadToEnd();
        //        }
        //        switch (obj.MessageType)
        //        {
        //            case (int)EmailMessageType.Registration:
        //                subject = "Registration Successful !";
        //                title = "Your Account No is " + obj.AccountNo;
        //                description = "Congrats ! Your Account is Now Online...Please Login to use the services !";
        //                break;
        //            //case (int)EmailMessageType.Payment:
        //            //    subject = "Your Payment has been received !";
        //            //    title = "Your Payment Reference No is " + obj.PayReferenceNo;
        //            //    description = "Your Account will be activated soon";
        //            //    break;
        //            //case (int)EmailMessageType.Activation:
        //            //    subject = "Your Account is Activated...";
        //            //    title = "Congrats ! Your Account is Now Online...";
        //            //    description = "Your can manage your account efficiently..Please Login to use the services !";
        //            //    break;
        //            case (int)EmailMessageType.LoginOTP:
        //                passcode = GenerateRandomNo().ToString();
        //                subject = "Your Login Password is :";
        //                title = passcode;
        //                description = "Please don't share to anyone";
        //                break;
        //        }

        //        body = body.Replace("{UserName}", obj.UserName);
        //        body = body.Replace("{Title}", title);
        //        body = body.Replace("{Description}", description);
        //        body = body.Replace("{ImgPath}", ImgPath);

        //        emailBody.Body = body;
        //        emailBody.Subject = subject;
        //        emailBody.Passcode = passcode;

        //        return emailBody;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}


    }
    public static class NextItem
    {
        public static T NextOf<T>(this IList<T> list, T item)
        {
            var indexOf = list.IndexOf(item);
            //return list[indexOf == list.Count - 1 ? 0 : indexOf + 1];

            var nextIndex = list.IndexOf(item) + 1;
            if (nextIndex == list.Count)
            {
                return list[0];
            }
            return list[nextIndex];
        }
    }

   
    public static class Constants
    {
        public static class Device
        {
            public const string PlatformInfoKey = nameof(PlatformInfoKey);
        }
    }

    //public static class EncryptionHelper
    //{
       
    //    private static readonly string EncryptionKey = "your-16-byte-key"; // AES requires a key size of 16, 24, or 32 bytes. Ensure this key is securely managed.

    //    // Encrypt a string value using AES encryption
    //    public static string Encrypt(string plainText)
    //    {
    //        using (var aesAlg = Aes.Create())
    //        {
    //            aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
    //            aesAlg.IV = new byte[16]; // Initialize IV with zeros (can be a random value, but for simplicity, we'll use zeros here)

    //            using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
    //            using (var msEncrypt = new MemoryStream())
    //            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
    //            using (var swEncrypt = new StreamWriter(csEncrypt))
    //            {
    //                swEncrypt.Write(plainText);
    //                return Convert.ToBase64String(msEncrypt.ToArray());
    //            }
    //        }
    //    }

    //    // Decrypt an encrypted string value
    //    public static string Decrypt(string cipherText)
    //    {
    //        using (var aesAlg = Aes.Create())
    //        {
    //            aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
    //            aesAlg.IV = new byte[16]; // Initialize IV with zeros (can be a random value, but for simplicity, we'll use zeros here)

    //            using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
    //            using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
    //            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
    //            using (var srDecrypt = new StreamReader(csDecrypt))
    //            {
    //                return srDecrypt.ReadToEnd();
    //            }
    //        }
    //    }
    //}
}