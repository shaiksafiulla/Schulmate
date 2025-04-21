using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class OrganizationService : IOrganizationService, IDisposable
    {
        private readonly APIContext _context;
        private readonly HostEnvironmentService _hostingEnvironment;
        private readonly S3Service _s3Service;

        public OrganizationService(APIContext context, HostEnvironmentService hostingEnvironment, S3Service s3Service)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _s3Service = s3Service;

        }

        public async Task<Organization> GetOrganization()
        {
            var cat = await _context.Organization.AsNoTracking().FirstOrDefaultAsync(x => x.IsActive == ((int)Shared.ActiveState.Active).ToString());
            if (cat != null)
            {
                return new Organization
                {
                    Id = cat.Id,
                    Name = cat.Name,
                    Theme = cat.Theme,
                    Address = cat.Address,
                    LogoPhotoPath = cat.LogoPhotoPath,
                    HeaderPhotoPath = cat.HeaderPhotoPath
                };
            }
            return new Organization
            {
                LogoPhotoPath = Shared.GetNoImagePath(),
                HeaderPhotoPath = Shared.GetNoImagePath()
            };
        }

        public async Task<ServiceResult> SaveOrganization(Organization model, int userId)
        {
            ServiceResult result = null;
            var isUpdate = model.Id > 0;
            var cat = isUpdate
                ? await _context.Organization.AsNoTracking().SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)Shared.ActiveState.Active).ToString())
                : new Organization
                {
                    IsActive = ((int)Shared.ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = userId
                };

            if (cat == null) return result;

            cat.Name = model.Name;
            cat.Theme = model.Theme;
            cat.Address = model.Address;
            cat.LastModifiedDate = DateTime.Now;
            cat.LastModifiedByUserId = userId;

            ProcessFile(model.LogoFile,  cat.LogoPhotoPath,  cat.LogoFileName);
            ProcessFile(model.HeaderFile,  cat.HeaderPhotoPath,  cat.HeaderFileName);

            if (!isUpdate)
            {
                _context.Organization.Add(cat);
            }

            var saveChangesResult = await _context.SaveChangesAsync();
            if (saveChangesResult != 0)
            {
                result = new ServiceResult
                {
                    StatusId = isUpdate ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added,
                    RecordId = cat.Id
                };
            }

            return result;
        }

        private async void ProcessFile(IFormFile file,  string photoPath, string fileName)
        {
            if (file != null)
            {
                if (!string.IsNullOrEmpty(photoPath) && !photoPath.Contains("noimage.png"))
                {
                    var filePath = _hostingEnvironment.GetFullUrlFromDbPath(photoPath);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                fileName = file.FileName;
              //  photoPath = Shared.ProcessUploadFile((int)UploadType.Organization, file, _hostingEnvironment.GetWebRootPath());
                photoPath = await _s3Service.UploadFileAsync((int)UploadType.Organization, file);
            }
            else
            {
                fileName = "noimage.png";
                photoPath = Path.Combine("Uploads", "NoImage", "noimage.png");
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}