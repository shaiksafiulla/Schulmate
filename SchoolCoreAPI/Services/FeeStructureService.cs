using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class FeeStructureService : IFeeStructureService
    {
        private readonly APIContext _context;
        public FeeStructureService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<VFeeStructure>> GetAllFeeStructure(int UserId)
        {
            var objuser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == UserId);
            if (objuser == null)
            {
                return new List<VFeeStructure>();
            }

            return objuser.UserType == ((int)UserType.SuperAdmin).ToString()
                ? await _context.VFeeStructure.AsNoTracking().ToListAsync()
                : await _context.VFeeStructure.AsNoTracking().Where(x => x.BranchId == objuser.BranchId).ToListAsync();
        }

        public async Task<VFeeStructure> ViewFeeStructure(int Id)
        {
            return await _context.VFeeStructure.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<FeeStructure> GetFeeStructure(int Id, int UserId)
        {
            var model = new FeeStructure
            {
                LstFeeStructureDetail = new List<FeeStructureDetail>(),
                LstFeeStructureInstallment = new List<FeeStructureInstallment>(),
                FeeTypeSheet = await _context.FeeType.AsNoTracking()
                    .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                    .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                    .ToListAsync(),
                BranchSheet = await GetBranches(),
                ClassSheet = new List<SelectListItem>(),
                ClassIds = new List<string>()
            };

            if (Id > 0)
            {
                var cat = await _context.FeeStructure.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
                if (cat != null)
                {
                    model.Id = cat.Id;
                    model.Title = cat.Title;
                    model.Description = cat.Description;
                    model.SessionYearId = cat.SessionYearId;
                    model.BranchId = cat.BranchId;
                    model.DiscountType = cat.DiscountType;
                    model.DueChargeType = cat.DueChargeType;
                    model.HasInstallment = cat.HasInstallment;

                    var divselectedItem = model.BranchSheet.Find(p => p.Value == cat.BranchId.ToString());
                    if (divselectedItem != null)
                    {
                        divselectedItem.Selected = true;
                        model.ClassIds = await _context.FeeStructureClass.AsNoTracking()
                            .Where(x => x.FeeStructureId == cat.Id && x.BranchId == cat.BranchId)
                            .Select(x => x.ClassId.ToString())
                            .ToListAsync();
                    }
                }
            }
            else
            {
                var objsession = await _context.SessionYear.AsNoTracking()
                    .SingleOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.IsDefault);
                if (objsession != null)
                {
                    model.SessionYearId = objsession.Id;
                }

                model.DiscountType = "1";
                model.DueChargeType = "1";
                model.HasInstallment = "1";
            }

            return model;
        }

        public async Task<bool> IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? await _context.ExamType.AsNoTracking().AnyAsync(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.ExamType.AsNoTracking().AnyAsync(e => e.Name == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<ServiceResult> SaveFeeStructure(FeeStructure model, int UserId)
        {
            ServiceResult result = null;
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (model.Id > 0)
                    {
                        var cat = await _context.FeeStructure.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                        if (cat != null)
                        {
                            cat.Title = model.Title;
                            cat.Description = model.Description;
                            cat.LastModifiedDate = DateTime.Now;
                            cat.LastModifiedByUserId = UserId;
                            _context.Entry(cat).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        var cat = new FeeStructure
                        {
                            Title = model.Title,
                            BranchId = model.BranchId,
                            SessionYearId = model.SessionYearId,
                            Description = model.Description,
                            TotalAmount = model.TotalAmount,
                            DueDate = model.DueDate,
                            DueChargeType = model.DueChargeType,
                            DueCharge = model.DueCharge,
                            HasInstallment = model.HasInstallment,
                            DiscountType = model.DiscountType,
                            Discount = model.Discount,
                            TotalAmountAfterDiscount = model.DiscountType == ((int)PercentFixedType.Percentage).ToString()
                                ? (model.TotalAmount * model.Discount) / 100
                                : model.TotalAmount - model.Discount,
                            IsActive = ((int)ActiveState.Active).ToString(),
                            CreatedDate = DateTime.Now,
                            CreatedByUserId = UserId
                        };
                        _context.FeeStructure.Add(cat);
                        await _context.SaveChangesAsync();

                        foreach (var assigncls in model.ClassIds)
                        {
                            var objbrcls = await _context.BranchClass.SingleOrDefaultAsync(x => x.BranchId == cat.BranchId && x.ClassId == int.Parse(assigncls));
                            if (objbrcls != null)
                            {
                                var fsc = new FeeStructureClass
                                {
                                    FeeStructureId = cat.Id,
                                    SessionYearId = cat.SessionYearId,
                                    BranchClassId = objbrcls.Id,
                                    BranchId = cat.BranchId,
                                    ClassId = int.Parse(assigncls),
                                    CreatedDate = DateTime.Now,
                                    CreatedByUserId = UserId
                                };
                                _context.FeeStructureClass.Add(fsc);
                            }
                        }
                        await _context.SaveChangesAsync();

                        foreach (var feedetail in model.LstFeeStructureDetail)
                        {
                            var detail = new FeeStructureDetail
                            {
                                FeeStructureId = cat.Id,
                                BranchId = cat.BranchId,
                                SessionYearId = cat.SessionYearId,
                                FeeTypeId = feedetail.FeeTypeId,
                                Amount = feedetail.Amount,
                                IsActive = ((int)ActiveState.Active).ToString(),
                                CreatedDate = DateTime.Now,
                                CreatedByUserId = UserId
                            };
                            _context.FeeStructureDetail.Add(detail);
                        }
                        await _context.SaveChangesAsync();

                        foreach (var cominstall in model.LstFeeStructureInstallment)
                        {
                            var install = new FeeStructureInstallment
                            {
                                InstallmentName = cominstall.InstallmentName,
                                FeeStructureId = cat.Id,
                                BranchId = cat.BranchId,
                                SessionYearId = cat.SessionYearId,
                                DueDate = cominstall.DueDate,
                                DueChargeType = cominstall.DueChargeType,
                                DueCharge = cominstall.DueCharge,
                                Amount = cominstall.Amount,
                                IsActive = ((int)ActiveState.Active).ToString(),
                                CreatedDate = DateTime.Now,
                                CreatedByUserId = UserId
                            };
                            _context.FeeStructureInstallment.Add(install);
                        }
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();
                        result = new ServiceResult
                        {
                            StatusId = (int)ServiceResultStatus.Added,
                            RecordId = cat.Id
                        };
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }
            return result;
        }

        public async Task<ServiceResult> DeleteFeeStructure(int Id, int UserId)
        {
            var cat = await _context.FeeStructure.SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return null;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;
            _context.Entry(cat).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new ServiceResult
            {
                StatusId = (int)ServiceResultStatus.Deleted,
                RecordId = cat.Id
            };
        }

        public async Task<List<SelectListItem>> GetBranches()
        {
            return await _context.Branch.AsNoTracking()
                .Where(x => x.IsActive == ((int)Shared.ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetClassesByBranch(int branchId, int sessionyearId)
        {
            var allclsids = await _context.BranchClass.AsNoTracking()
                .Where(x => x.BranchId == branchId)
                .Select(x => x.ClassId)
                .ToListAsync();

            var allexistclsids = await _context.FeeStructureClass.AsNoTracking()
                .Where(x => x.BranchId == branchId && x.SessionYearId == sessionyearId)
                .Select(x => x.ClassId)
                .ToListAsync();

            var allnonexistclsids = allclsids.Except(allexistclsids);

            return await _context.Class.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && allnonexistclsids.Contains(x.Id))
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public async Task Dispose()
        {
            await _context.DisposeAsync();
        }
    }
}
