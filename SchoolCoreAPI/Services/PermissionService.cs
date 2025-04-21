using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly APIContext _context;
        private readonly IConfiguration _config;
        private readonly IVapidKeyService _vapidKeyService;
        private readonly string _connectionString;

        public PermissionService(APIContext context, IConfiguration config, IVapidKeyService vapidKeyService)
        {
            _context = context;
            _config = config;
            _vapidKeyService = vapidKeyService;
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<List<VRoles>> GetAllRoles()
        {
            return await _context.VRoles.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<Role> GetRole(int Id)
        {
            var model = new Role
            {
                BranchSheet = await GetBranches(),
                SessionYearSheet = await GetSessionYears()
            };

            if (Id > 0)
            {
                var cat = await _context.Role.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
                if (cat != null)
                {
                    model.Id = cat.Id;
                    model.Name = cat.Name;
                    model.Description = cat.Description;
                    model.BranchId = cat.BranchId;
                    model.SessionYearId = cat.SessionYearId;

                    var divselectedItem = model.BranchSheet.FirstOrDefault(p => p.Value == cat.BranchId.ToString());
                    if (divselectedItem != null)
                    {
                        divselectedItem.Selected = true;
                    }

                    divselectedItem = model.SessionYearSheet.FirstOrDefault(p => p.Value == cat.SessionYearId.ToString());
                    if (divselectedItem != null)
                    {
                        divselectedItem.Selected = true;
                    }
                }
            }

            return model;
        }

        public async Task<ServiceResult> SaveRole(Role model, int UserId)
        {
            ServiceResult result = null;
            if (model.Id > 0)
            {
                var role = await _context.Role.AsNoTracking().SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                if (role != null)
                {
                    role.Name = model.Name;
                    role.Description = model.Description;
                    role.BranchId = model.BranchId;
                    role.SessionYearId = model.SessionYearId;
                    role.ModifiedDate = DateTime.Now;
                    role.ModifiedByUserId = UserId;
                    _context.Entry(role).State = EntityState.Modified;
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        result = new ServiceResult
                        {
                            StatusId = (int)ServiceResultStatus.Updated,
                            RecordId = role.Id
                        };
                    }
                }
            }
            else
            {
                var role = new Role
                {
                    Name = model.Name,
                    Description = model.Description,
                    BranchId = model.BranchId,
                    SessionYearId = model.SessionYearId,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId
                };
                _context.Role.Add(role);
                _context.Entry(role).State = EntityState.Added;
                if (await _context.SaveChangesAsync() > 0)
                {
                    result = new ServiceResult
                    {
                        StatusId = (int)ServiceResultStatus.Added,
                        RecordId = role.Id
                    };
                }
            }
            return result;
        }

        public async Task<VRoles> ViewRole(int Id)
        {
            return await _context.VRoles.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<ServiceResult> DeleteRole(int Id, int UserId)
        {
            var result = new ServiceResult();
            var role = await _context.Role.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
            if (role != null)
            {
                role.IsActive = ((int)ActiveState.InActive).ToString();
                role.ModifiedByUserId = UserId;
                role.ModifiedDate = DateTime.Now;
                _context.Entry(role).State = EntityState.Modified;
                if (await _context.SaveChangesAsync() > 0)
                {
                    result.StatusId = (int)ServiceResultStatus.Deleted;
                    result.RecordId = role.Id;
                }
            }
            return result;
        }

        public async Task<bool> IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? await _context.Role.AsNoTracking().AnyAsync(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.Role.AsNoTracking().AnyAsync(e => e.Name == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<List<SelectListItem>> GetBranches()
        {
            return await _context.Branch.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetSessionYears()
        {
            return await _context.SessionYear.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public async Task<PageVM> GetRolePages(int Id)
        {
            var objrole = await _context.Role.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            var lstPage = await _context.VUserTypePage.AsNoTracking().Where(x => x.UserType == objrole.RoleType).OrderByDescending(x => x.Id).ToListAsync();
            if (lstPage != null && lstPage.Count > 0 && Id > 0)
            {
                var lstrolepage = _context.RolePage.AsNoTracking().Where(x => x.RoleId == Id).ToList();
                foreach (var item in lstPage)
                {
                    item.IsSelected = lstrolepage.Any(rolepage => item.Id == rolepage.UserTypePageId);
                }
            }
            return new PageVM { LstUserTypePage = lstPage, RoleId = Id };
        }

        public async Task<ServiceResult> UpdateRolePage(PageVM model, int UserId)
        {
            var result = new ServiceResult();
            var lstrolepage = await _context.RolePage.AsNoTracking().Where(x => x.RoleId == model.RoleId).ToListAsync();
            _context.RolePage.RemoveRange(lstrolepage);

            var selectedPages = model.LstUserTypePage.Where(x => x.IsSelected).ToList();
            if (selectedPages.Count > 0)
            {
                foreach (var item in selectedPages)
                {
                    var rolpage = new RolePage
                    {
                        RoleId = (int)model.RoleId,
                        UserTypePageId = item.Id,
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = UserId
                    };
                    _context.RolePage.Add(rolpage);
                    _context.Entry(rolpage).State = EntityState.Added;
                }
            }
            if (await _context.SaveChangesAsync() > 0)
            {
                result.StatusId = (int)ServiceResultStatus.Updated;
                result.RecordId = (int)model.RoleId;
            }
            return result;
        }

        public async Task<RoleUserVM> GetRoleUsers(int Id)
        {
            var lstUser = await _context.userinfo.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString()).OrderByDescending(x => x.Id).ToListAsync();
            if (lstUser != null && lstUser.Count > 0 && Id > 0)
            {
                var lstroleuser = await  _context.UserRole.AsNoTracking().Where(x => x.RoleId == Id).ToListAsync();
                foreach (var item in lstUser)
                {
                    item.IsSelected = lstroleuser.Any(roleuser => item.Id == roleuser.UserId);
                }
            }
            return new RoleUserVM { LstUser = lstUser, RoleId = Id };
        }

        public async Task<ServiceResult> UpdateRoleUsers(RoleUserVM model, int UserId)
        {
            var result = new ServiceResult();
            if (model != null && model.LstUser.Count > 0)
            {
                var lstrolepage = await _context.UserRole.AsNoTracking().Where(x => x.RoleId == model.RoleId).ToListAsync();
                _context.UserRole.RemoveRange(lstrolepage);

                var selectedUsers = model.LstUser.Where(x => x.IsSelected).ToList();
                if (selectedUsers.Count > 0)
                {
                    foreach (var item in selectedUsers)
                    {
                        var usrRole = new UserRole
                        {
                            RoleId = model.RoleId,
                            UserId = item.Id,
                            CreatedDate = DateTime.Now,
                            CreatedByUserId = UserId
                        };
                        _context.UserRole.Add(usrRole);
                        _context.Entry(usrRole).State = EntityState.Added;
                    }
                }
                if (await _context.SaveChangesAsync() > 0)
                {
                    result.StatusId = (int)ServiceResultStatus.Updated;
                    result.RecordId = model.RoleId;
                }
            }
            return result;
        }

        public async Task<List<VUserInfo>> GetAllUsers()
        {
            return await _context.VUserInfo.AsNoTracking().Where(x => x.UserType != ((int)UserType.SuperAdmin).ToString()).OrderByDescending(x => x.Id).ToListAsync();
        }

        public async Task<UserVM> GetAllUsersByPage(UserVM usrVM)
        {
            const int maxRows = 5;
            var lstcount = _context.VUserInfo.AsNoTracking().Count(x => x.UserType != ((int)UserType.SuperAdmin).ToString());
            var lst = await _context.VUserInfo.AsNoTracking()
                .Where(x => x.UserType != ((int)UserType.SuperAdmin).ToString())
                .OrderByDescending(x => x.Id)
                .Skip((usrVM.CurrentPageIndex - 1) * maxRows)
                .Take(maxRows)
                .ToListAsync();

            var pageCount = (int)Math.Ceiling((double)lstcount / maxRows);
            return new UserVM
            {
                LstUser = lst,
                PageCount = pageCount,
                CurrentPageIndex = usrVM.CurrentPageIndex,
                TotalCount = lstcount
            };
        }

        public async Task<VUserInfo> ViewUser(int Id)
        {
            return await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<userinfo> GetUser(int Id)
        {
            var usr = await _context.userinfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (usr != null)
            {
                usr.HasLoginSelected = usr.HasLogin == ((int)ActiveState.Active).ToString();
            }
            return usr;
        }

        public async Task<ServiceResult> SaveUser(userinfo model, int UserId)
        {
            ServiceResult result = null;
            if (model.Id > 0)
            {
                var usr = await _context.userinfo.AsNoTracking().SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                if (usr != null)
                {
                    usr.UserName = model.UserName;
                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        usr.Password = Shared.ToSHA2569(model.NewPassword);
                    }
                    usr.HasLogin = model.HasLoginSelected ? ((int)ActiveState.Active).ToString() : ((int)ActiveState.InActive).ToString();
                    usr.ModifiedDate = DateTime.Now;
                    usr.ModifiedByUserId = UserId;
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        result = new ServiceResult
                        {
                            StatusId = (int)ServiceResultStatus.Updated,
                            RecordId = model.Id
                        };
                    }
                }
            }
            return result;
        }

        public async Task<UserRoleVM> GetUserRole(int Id)
        {
            var lstRole = await _context.Role.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString()).OrderByDescending(x => x.Id).ToListAsync();
            if (lstRole != null && lstRole.Count > 0 && Id > 0)
            {
                var lstuserrole = _context.UserRole.Where(x => x.UserId == Id).ToList();
                foreach (var item in lstRole)
                {
                    item.IsSelected = lstuserrole.Any(userrole => item.Id == userrole.RoleId);
                }
            }
            return new UserRoleVM { LstRole = lstRole, UserId = Id };
        }

        public async Task<ServiceResult> UpdateUserRole(UserRoleVM model, int UserId)
        {
            var result = new ServiceResult();
            if (model != null && model.LstRole.Count > 0)
            {
                var lstrolepage = await _context.UserRole.AsNoTracking().Where(x => x.UserId == model.UserId).ToListAsync();
                _context.UserRole.RemoveRange(lstrolepage);

                var selectedRoles = model.LstRole.Where(x => x.IsSelected).ToList();
                if (selectedRoles.Count > 0)
                {
                    foreach (var item in selectedRoles)
                    {
                        var usrRole = new UserRole
                        {
                            UserId = model.UserId,
                            RoleId = item.Id,
                            CreatedDate = DateTime.Now,
                            CreatedByUserId = UserId
                        };
                        _context.UserRole.Add(usrRole);
                        _context.Entry(usrRole).State = EntityState.Added;
                    }
                }
                if (await _context.SaveChangesAsync() > 0)
                {
                    result.StatusId = (int)ServiceResultStatus.Updated;
                    result.RecordId = model.UserId;
                }
            }
            return result;
        }

        public async Task<int> IsLoginValid(LoginUser user)
        {
            int index = 0;
            try
            {
                var crypto = Shared.ToSHA2569(user.Password);
                var obj = await _context.userinfo.SingleOrDefaultAsync(m =>
                    m.UserName.Trim().ToUpper() == user.UserName.Trim().ToUpper() &&
                    m.Password == crypto &&
                    (m.IsActive == ((int)ActiveState.Active).ToString() || m.IsActive == ((int)ActiveState.ActiveAdmin).ToString()) &&
                    m.HasLogin == ((int)ActiveState.Active).ToString());

                if (obj != null)
                {
                    obj.LastLoggedIn = DateTime.Now.ToUniversalTime();
                    obj.ModifiedDate = DateTime.Now.ToUniversalTime();
                    obj.ModifiedByUserId = obj.Id;
                    index = await _context.SaveChangesAsync();
                    if (index > 0)
                    {
                        index = obj.Id;
                    }
                }
            }
            catch (Exception ex)
            {

               // throw;
            }
            
            return index;
        }

        public static void ListControllersAndActions()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var writer = new StreamWriter("D:\\apicontrollermethods.txt"))
            {
                var controllers = assembly.GetTypes().Where(t => t.IsClass && t.IsSubclassOf(typeof(ControllerBase)));
                foreach (var controller in controllers)
                {
                    writer.WriteLine($"Controller: {controller.Name}");
                    var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                        .Where(m => m.ReturnType.IsGenericType);
                    if (!methods.Any())
                    {
                        writer.WriteLine("  No action methods found.");
                    }
                    foreach (var method in methods)
                    {
                        writer.WriteLine($"  Action: {method.Name}");
                    }
                    writer.WriteLine();
                }
            }
        }

        public async Task<VUserInfo> ValidateUserLogin(LoginUser user)
        {
            var crypto = Shared.ToSHA2569(user.Password);
            VUserInfo objUser = null;
            try
            {
                var result = await _context.userinfo.AsNoTracking().SingleOrDefaultAsync(m =>
                    m.UserName == user.UserName &&
                    m.Password == crypto &&
                    (m.IsActive == ((int)ActiveState.Active).ToString() || m.IsActive == ((int)ActiveState.ActiveAdmin).ToString()) &&
                    m.HasLogin == ((int)ActiveState.Active).ToString());

                if (result != null)
                {
                    result.LastLoggedIn = DateTime.Now;
                    result.ModifiedDate = DateTime.Now;
                    result.ModifiedByUserId = result.Id;
                    if (_context.SaveChanges() > 0)
                    {
                        objUser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(m => m.Id == result.Id);
                        if (objUser != null)
                        {
                            objUser.LstPages = _context.vuserpermissions.AsNoTracking().Where(x => x.UserId == objUser.Id).Select(x => x.Screen).ToList();
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Handle exception
            }
            return objUser;
        }

        public async Task<VUserInfo> GetUserAccessById(int userId)
        {
            try
            {
                var user = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(m => m.Id == userId);
                if (user != null)
                {
                    user.LstPages = await _context.vuserpermissions.AsNoTracking().Where(x => x.UserId == user.Id).Select(x => x.Screen).ToListAsync();
                    user.NotiCount = await _context.VNotification.AsNoTracking().CountAsync(x => x.ToUserId == userId && x.SessionYearId == user.SessionYearId && x.ReadStatusId == ((int)ReadStatus.UnRead).ToString());
                    var (publicKey, _) = _vapidKeyService.GetKeysFromAppSettings();
                    user.Vapidkey = publicKey;
                }
                return user;
            }
            catch (Exception)
            {
                // Handle exception
            }
            return null;
        }

        public async Task<string> GetToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                null,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<userinfo> GetUserById(int userId)
        {
            return await _context.userinfo.AsNoTracking().SingleOrDefaultAsync(m => m.Id == userId && m.IsActive != ((int)ActiveState.InActive).ToString());
        }

        public async Task<int> ChangePassword(ChangePwd model, int UserId)
        {
            var userexist = await _context.userinfo.AsNoTracking().SingleOrDefaultAsync(m =>
                m.Id == UserId &&
                m.IsActive != ((int)ActiveState.InActive).ToString() &&
                m.Password == Shared.ToSHA2569(model.Password));

            if (userexist != null)
            {
                userexist.Password = Shared.ToSHA2569(model.NewPassword);
                userexist.ModifiedByUserId = userexist.Id;
                userexist.ModifiedDate = DateTime.Now;
                return await _context.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<PageFunctionProcedure> GetPageFunctions(int UserId)
        {
            var result = new PageFunctionProcedure();
            try
            {
                // Assuming DataTableToJSON is a method that converts DataTable to JSON string
                result.StrResult = DataTableToJSON(new DataTable());
            }
            catch (Exception)
            {
                // Handle exception
            }
            return result;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
