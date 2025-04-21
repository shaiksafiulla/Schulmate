using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IAttachmentService
    {
        Task<List<VAttachment>> GetAllAttachment(int type, int referid);
        Task<VAttachment> ViewAttachment(int Id);
        Task<Attachment> GetAttachment(int Id);
        Task<ServiceResult> SaveAttachment(Attachment model, int UserId);
        Task<ServiceResult> DeleteAttachment(int Id, int UserId);
        Task<Tuple<byte[], string>> GetFileBytes(int Id);
        Task<bool> IsRecordExists(string name, int Id);
        void Dispose();
    }

   
}
