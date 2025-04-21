using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class ReportSettingsService : IReportSettingsService
    {
        private readonly APIContext _context;
        private readonly HostEnvironmentService _hostingEnvironment;
        private readonly S3Service _s3Service;

        public ReportSettingsService(APIContext context, HostEnvironmentService hostingEnvironment, S3Service s3Service)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _s3Service = s3Service;
        }

        public async Task<ReportSettings> GetReportSettings(int UserId)
        {
            var settings = new ReportSettings();
            try
            {
                var objUser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == UserId);
                if (objUser == null) return settings;

                settings.LstPreparedBy = GetEmployees();
                settings.LstVerifiedBy = GetEmployees();

                var reportSettings = await _context.ReportSettings.AsNoTracking().FirstOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.BranchId == objUser.BranchId);
                if (reportSettings != null)
                {
                    CopyReportSettings(reportSettings, settings);
                    SetSelectedItems(settings);
                }
                else
                {
                    SetDefaultSettings(settings, (int)objUser.BranchId);
                }
            }
            catch (Exception)
            {
                // Log exception
            }
            return settings;
        }

        public async Task<ReportSettings> GetReportSettingsByBranch(int BranchId)
        {
            var settings = new ReportSettings();
            try
            {
                settings.LstPreparedBy = GetEmployees();
                settings.LstVerifiedBy = GetEmployees();

                var reportSettings = await _context.ReportSettings.AsNoTracking().FirstOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.BranchId == BranchId);
                if (reportSettings != null)
                {
                    CopyReportSettings(reportSettings, settings);
                    SetSelectedItems(settings);
                }
                else
                {
                    SetDefaultSettings(settings, BranchId);
                }
            }
            catch (Exception)
            {
                // Log exception
            }
            return settings;
        }

        public async Task<ServiceResult> SaveReportSettings(ReportSettings model, int UserId)
        {
            ServiceResult result = null;
            try
            {
                if (model.Id > 0)
                {
                    var settings = await _context.ReportSettings.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                    if (settings != null)
                    {
                        var rptsettings = await UpdateReportSettings(settings, model, UserId);
                        rptsettings.ModifiedDate = DateTime.Now;
                        rptsettings.ModifiedByUserId = UserId;

                        _context.Entry(rptsettings).State = EntityState.Modified;
                        if (await _context.SaveChangesAsync() != 0)
                        {
                            result = new ServiceResult
                            {
                                StatusId = (int)ServiceResultStatus.Updated,
                                RecordId = rptsettings.Id
                            };
                        }
                    }
                }
                else
                {
                    var settings = new ReportSettings();
                    var rptsettings = await UpdateReportSettings(settings, model, UserId);
                    rptsettings.CreatedDate = DateTime.Now;
                    rptsettings.CreatedByUserId = UserId;
                    rptsettings.IsActive = ((int)ActiveState.Active).ToString();
                    _context.ReportSettings.Add(rptsettings);
                    _context.Entry(rptsettings).State = EntityState.Added;
                    if (await _context.SaveChangesAsync() != 0)
                    {
                        result = new ServiceResult
                        {
                            StatusId = (int)ServiceResultStatus.Added,
                            RecordId = rptsettings.Id
                        };
                    }

                    //var saveResult = await _context.SaveChangesAsync();
                    //if (saveResult != 0)
                    //{
                    //    result = new ServiceResult
                    //    {
                    //        StatusId = model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added,
                    //        RecordId = model.Id
                    //    };
                    //}
                }
            }
            catch (Exception ex)
            {
                // Log exception
            }
            return result;
        }

        private void CopyReportSettings(ReportSettings source, ReportSettings target)
        {
            target.Id = source.Id;
            target.BranchId = source.BranchId;
            target.AFourHeaderType = source.AFourHeaderType;
            target.AFourFooterType = source.AFourFooterType;
            target.AFourHeaderSpace = source.AFourHeaderSpace;
            target.AFourFooterSpace = source.AFourFooterSpace;
            target.AFiveHeaderType = source.AFiveHeaderType;
            target.AFiveFooterType = source.AFiveFooterType;
            target.AFiveHeaderSpace = source.AFiveHeaderSpace;
            target.AFiveFooterSpace = source.AFiveFooterSpace;
            target.BarcodeType = source.BarcodeType;
            target.EsignType = source.EsignType;
            target.PreparedById = source.PreparedById;
            target.VerifiedById = source.VerifiedById;
            target.AFourHeaderPhotoPath = source.AFourHeaderPhotoPath;
            target.AFourFooterPhotoPath = source.AFourFooterPhotoPath;
            target.AFiveHeaderPhotoPath = source.AFiveHeaderPhotoPath;
            target.AFiveFooterPhotoPath = source.AFiveFooterPhotoPath;
        }

        private void SetSelectedItems(ReportSettings settings)
        {
            if (settings.PreparedById > 0)
            {
                var selectedItem = settings.LstPreparedBy.Find(p => p.Value == settings.PreparedById.ToString());
                if (selectedItem != null)
                {
                    selectedItem.Selected = true;
                }
            }
            if (settings.VerifiedById > 0)
            {
                var vselectedItem = settings.LstVerifiedBy.Find(p => p.Value == settings.VerifiedById.ToString());
                if (vselectedItem != null)
                {
                    vselectedItem.Selected = true;
                }
            }
        }

        private void SetDefaultSettings(ReportSettings settings, int branchId)
        {
            settings.AFourHeaderType = ((int)ActiveState.Active).ToString();
            settings.AFourFooterType = ((int)ActiveState.Active).ToString();
            settings.AFiveHeaderType = ((int)ActiveState.Active).ToString();
            settings.AFiveFooterType = ((int)ActiveState.Active).ToString();
            settings.BarcodeType = ((int)ActiveState.Active).ToString();
            settings.EsignType = ((int)ActiveState.Active).ToString();
            settings.AFourHeaderPhotoPath = Shared.GetNoImagePath();
            settings.AFourFooterPhotoPath = Shared.GetNoImagePath();
            settings.AFiveHeaderPhotoPath = Shared.GetNoImagePath();
            settings.AFiveFooterPhotoPath = Shared.GetNoImagePath();
            settings.BranchId = branchId;
        }

        private async Task<ReportSettings> UpdateReportSettings(ReportSettings settings, ReportSettings model, int userId)
        {
            settings.BranchId = model.BranchId;
            settings.AFourHeaderType = model.AFourHeaderType;
            settings.AFourFooterType = model.AFourFooterType;
            settings.AFourHeaderSpace = model.AFourHeaderSpace;
            settings.AFourFooterSpace = model.AFourFooterSpace;
            settings.AFiveHeaderType = model.AFiveHeaderType;
            settings.AFiveFooterType = model.AFiveFooterType;
            settings.AFiveHeaderSpace = model.AFiveHeaderSpace;
            settings.AFiveFooterSpace = model.AFiveFooterSpace;
            settings.BarcodeType = model.BarcodeType;
            settings.EsignType = model.EsignType;
            settings.PreparedById = model.PreparedById;
            settings.VerifiedById = model.VerifiedById;
            

            var afourtupleheader= await ProcessSettingFileUpload(model.Id, settings.AFourHeaderPhotoPath, model.AFourHeaderFile);
            if(afourtupleheader != null)
            {
                settings.AFourHeaderFileName = afourtupleheader.Item1 ?? string.Empty;
                settings.AFourHeaderPhotoPath = afourtupleheader.Item2 ?? string.Empty;
            }          

            var afourtuplefooter = await ProcessSettingFileUpload(model.Id, settings.AFourFooterPhotoPath, model.AFourFooterFile);
            if(afourtuplefooter != null)
            {
                settings.AFourFooterFileName = afourtuplefooter.Item1 ?? string.Empty;
                settings.AFourFooterPhotoPath = afourtuplefooter.Item2 ?? string.Empty; ;
            }
            

            var afivetupleheader = await ProcessSettingFileUpload(model.Id, settings.AFiveHeaderPhotoPath, model.AFiveHeaderFile);
            if(afivetupleheader != null)
            {
                settings.AFiveHeaderFileName = afivetupleheader.Item1 ?? string.Empty;
                settings.AFiveHeaderPhotoPath = afivetupleheader.Item2 ?? string.Empty;
            }
            

            var afivetuplefooter = await ProcessSettingFileUpload(model.Id, settings.AFiveFooterPhotoPath, model.AFiveFooterFile);
            if(afivetuplefooter != null)
            {
                settings.AFiveFooterFileName = afivetuplefooter.Item1 ?? string.Empty;
                settings.AFiveFooterPhotoPath = afivetuplefooter.Item2 ?? string.Empty;
            }
            return settings;
            //ProcessFileUpload(model.AFourHeaderFile,  settings.AFourHeaderPhotoPath,  settings.AFourHeaderFileName);
            //ProcessFileUpload(model.AFourFooterFile,  settings.AFourFooterPhotoPath,  settings.AFourFooterFileName);
            //ProcessFileUpload(model.AFiveHeaderFile,  settings.AFiveHeaderPhotoPath,  settings.AFiveHeaderFileName);
            //ProcessFileUpload(model.AFiveFooterFile,  settings.AFiveFooterPhotoPath,  settings.AFiveFooterFileName);
        }        
        private async Task<Tuple<string,string>> ProcessSettingFileUpload(int Id, string existfilePath, IFormFile modelFile)
        {
            Tuple<string, string> result = null;
            if (Id > 0)
            {
                if (modelFile != null)
                {
                    //if (existfilePath != null && !existfilePath.Contains("noimage.png"))
                    //{
                    //    var filePath = _hostingEnvironment.GetFullUrlFromDbPath(existfilePath);
                    //    if (File.Exists(filePath))
                    //    {
                    //        File.Delete(filePath);
                    //    }
                    //}
                    if (!string.IsNullOrEmpty(existfilePath) && !existfilePath.Contains("noimage.png"))
                    {
                        await _s3Service.DeleteFileAsync(existfilePath);
                    }
                    var newpath = await _s3Service.UploadFileAsync((int)UploadType.ReportSettings, modelFile);

                    result = new Tuple<string, string>(modelFile.FileName, newpath);                   
                }
            }
            else
            {
                if (modelFile != null)
                {
                    var newpath = await _s3Service.UploadFileAsync((int)UploadType.ReportSettings, modelFile);
                    result = new Tuple<string, string>(modelFile.FileName, newpath);
                    // result = new Tuple<string, string>(modelFile.FileName, Shared.ProcessUploadFile((int)UploadType.ReportSettings, modelFile, _hostingEnvironment.GetWebRootPath()));
                }
                else
                {
                    result = new Tuple<string, string>("noimage.png", Path.Combine("/uploads", "NoImage", "noimage.png"));                   
                }

            }
            return result;
            //if (file != null)
            //{
            //    if (!string.IsNullOrEmpty(photoPath) && !photoPath.Contains("noimage.png"))
            //    {
            //        var filePath = _hostingEnvironment.GetFullUrlFromDbPath(photoPath);
            //        if (File.Exists(filePath))
            //        {
            //            File.Delete(filePath);
            //        }
            //    }
            //    fileName = file.FileName;
            //    photoPath = Shared.ProcessUploadFile((int)UploadType.ReportSettings, file, _hostingEnvironment.GetWebRootPath());
            //}
        }

        public List<SelectListItem> GetEmployees()
        {
            return _context.VTeachers.Select(x => new SelectListItem { Text = x.FullName, Value = x.Id.ToString() }).ToList();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}