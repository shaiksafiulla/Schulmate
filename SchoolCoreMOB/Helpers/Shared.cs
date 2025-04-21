
using SchoolCoreMOB.Models;

namespace SchoolCoreMOB.Helpers
{
    public class Shared
    {
        public enum AttendanceStatus
        {
            Present = 1,
            Absent = 0,
            Leave = 2
        }

        public enum ReadStatus
        {
            UnRead = 1, Read = 0
        }
        public enum UserType
        {
            SuperAdmin = 1, Admin = 2, Teacher = 3, Student = 4, Staff = 5, Driver = 6
        }
        public enum TransferType
        {
            Admitted = 1, Transfer = 2, Promoted = 3, Leave = 4
        }
        public enum AssignmentStatus
        {
            New = 0, Accepted = 1, Rejected = 2,
        }
        public enum LeaveRequestStatus
        {
            UnderProcess = 1, Rejected = 2, Approved = 3
        }
        public enum AttachmentType
        {
            student = 1, teacher = 2, admin = 3, staff = 4, driver = 5, questionPaper = 6, organization = 7
        }        
    }
    //public static class UserExtensions
    //{
    //    public static Fido2User ToFido2User(this User user)
    //    {
    //        return new Fido2User
    //        {
    //            Id = user.Id.ToByteArray(),
    //            Name = user.UserName,
    //            DisplayName = user.DisplayName
    //        };
    //    }
    //}
    public static class Constants
    {
        public static class Device
        {
            public const string PlatformInfoKey = nameof(PlatformInfoKey);
        }
    }
    //public static class SessionExtensions
    //{
    //    public static void SetObjectAsJson(this ISession session, string key, object value)
    //    {
    //        session.SetString(key, JsonConvert.SerializeObject(value));
    //    }

    //    public static T GetObjectFromJson<T>(this ISession session, string key)
    //    {
    //        var value = session.GetString(key);
    //        return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
    //    }
    //}
}
