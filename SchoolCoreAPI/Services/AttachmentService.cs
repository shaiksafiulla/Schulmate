using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class AttachmentService : IAttachmentService, IDisposable
    {
        private readonly APIContext _context;
        private readonly HostEnvironmentService _hostingEnvironment;
        private readonly S3Service _s3Service;

        public AttachmentService(APIContext context, HostEnvironmentService hostingEnvironment, S3Service s3Service)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _s3Service = s3Service;
        }

        public async Task<List<VAttachment>> GetAllAttachment(int type, int referid)
        {
            try
            {
                return await _context.VAttachment
                    .Where(x => x.AttachType == type.ToString() && x.ReferenceId == referid)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<VAttachment> ViewAttachment(int Id)
        {
            return await _context.VAttachment.SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<Attachment> GetAttachment(int Id)
        {
            if (Id <= 0)
            {
                return new Attachment
                {
                    FilePath = Shared.GetNoImagePath(),
                    FileType = "1"
                };
            }

            var cat = await _context.Attachment.SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)Shared.ActiveState.Active).ToString());
            if (cat == null) return new Attachment();

            return new Attachment
            {
                Id = cat.Id,
                Name = cat.Name,
                AttachType = cat.AttachType,
                ReferenceId = cat.ReferenceId,
                Description = cat.Description,
                FileType = cat.FileType,
                FilePath = cat.FilePath,
                FileName = cat.FileName
            };
        }

        public async Task<Tuple<byte[], string>> GetFileBytes(int Id)
        {
            var objattach = await _context.Attachment.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id && m.IsActive == ((int)ActiveState.Active).ToString());
            if (objattach == null || string.IsNullOrEmpty(objattach.FilePath)) return null;

            //string filePath = _hostingEnvironment.GetFullUrlFromDbPath(objattach.FilePath);
            //var bytes = await File.ReadAllBytesAsync(filePath);
            //return new Tuple<byte[], string>(bytes, objattach.FileName);

            var bytes = await _s3Service.GetS3ObjectAsync(objattach.FilePath);
            return new Tuple<byte[], string>(bytes, objattach.FileName);
           
        }

        public async Task<bool> IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? await _context.Attachment.AnyAsync(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.Attachment.AnyAsync(e => e.Name == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<ServiceResult> SaveAttachment(Attachment model, int UserId)
        {
            var result = new ServiceResult();
            var now = DateTime.Now.ToUniversalTime();

            if (model.Id > 0)
            {
                var cat = await _context.Attachment.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)Shared.ActiveState.Active).ToString());
                if (cat == null) return null;

                cat.Name = model.Name;
                cat.FileType = model.FileType;
                cat.Description = model.Description;
                cat.LastModifiedDate = now;
                cat.LastModifiedByUserId = UserId;

                if (model.File != null)
                {
                    //if (!string.IsNullOrEmpty(cat.FilePath) && !cat.FilePath.Contains("noimage.png"))
                    //{
                    //    string filePath = _hostingEnvironment.GetFullUrlFromDbPath(cat.FilePath);
                    //    if (File.Exists(filePath))
                    //    {
                    //        File.Delete(filePath);
                    //    }
                    //}
                    if (!string.IsNullOrEmpty(cat.FilePath) && !cat.FilePath.Contains("noimage.png"))
                    {
                        await _s3Service.DeleteFileAsync(cat.FilePath);
                    }
                    cat.FileName = model.File.FileName;
                    //cat.FilePath = Shared.ProcessUploadFile((int)UploadType.Attachment, model.File, _hostingEnvironment.GetWebRootPath());
                    cat.FilePath = await _s3Service.UploadFileAsync((int)UploadType.Attachment, model.File);
                }

                if (await _context.SaveChangesAsync() > 0)
                {
                    result.StatusId = (int)ServiceResultStatus.Updated;
                    result.RecordId = cat.Id;
                }
            }
            else
            {
                var cat = new Attachment
                {
                    Name = model.Name,
                    Description = model.Description,
                    FileType = model.FileType,
                    AttachType = model.AttachType,
                    ReferenceId = model.ReferenceId,
                    IsActive = ((int)Shared.ActiveState.Active).ToString(),
                    CreatedDate = now,
                    CreatedByUserId = UserId,
                    LastModifiedDate = now,
                    LastModifiedByUserId = UserId,
                    FileName = model.File?.FileName ?? "noimage.png",
                    FilePath = model.File != null
                        //? Shared.ProcessUploadFile((int)UploadType.Attachment, model.File, _hostingEnvironment.GetWebRootPath())
                        ? await _s3Service.UploadFileAsync((int)UploadType.Attachment, model.File)
                        : Path.Combine("Uploads", "NoImage", "noimage.png")
                };

                _context.Attachment.Add(cat);
                if (await _context.SaveChangesAsync() > 0)
                {
                    result.StatusId = (int)ServiceResultStatus.Added;
                    result.RecordId = cat.Id;
                }
            }

            return result;
        }

        public async Task<ServiceResult> DeleteAttachment(int Id, int UserId)
        {
            var cat = await _context.Attachment.SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return null;

            cat.IsActive = ((int)Shared.ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;

            _context.Entry(cat).State = EntityState.Modified;
            if (await _context.SaveChangesAsync() > 0)
            {
                return new ServiceResult
                {
                    StatusId = (int)ServiceResultStatus.Deleted,
                    RecordId = cat.Id
                };
            }

            return null;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}