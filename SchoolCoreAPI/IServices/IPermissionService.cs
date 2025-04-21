using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IPermissionService
    {
        Task<List<VRoles>> GetAllRoles();
        //   RoleVM GetAllRolesByPage(RoleVM rolVM);
        Task<VRoles> ViewRole(int Id);
        Task<Role> GetRole(int Id);
        Task<ServiceResult> SaveRole(Role model, int UserId);
        Task<ServiceResult> DeleteRole(int Id, int UserId);
        Task<bool> IsRecordExists(string name, int Id);
        Task<PageVM> GetRolePages(int Id);
        Task<ServiceResult> UpdateRolePage(PageVM model, int UserId);
        Task<RoleUserVM> GetRoleUsers(int Id);
        Task<ServiceResult> UpdateRoleUsers(RoleUserVM model, int UserId);
        Task<List<VUserInfo>> GetAllUsers();
        Task<UserVM> GetAllUsersByPage(UserVM usrVM);
        Task<VUserInfo> ViewUser(int Id);
        Task<userinfo> GetUser(int Id);
        Task<ServiceResult> SaveUser(userinfo model, int UserId);
        Task<UserRoleVM> GetUserRole(int Id);
        Task<ServiceResult> UpdateUserRole(UserRoleVM model, int UserId);
        Task<int> IsLoginValid(LoginUser user);
        Task<string> GetToken(string username);
        Task<VUserInfo> GetUserAccessById(int userId);
        Task<VUserInfo> ValidateUserLogin(LoginUser user);
        Task<userinfo> GetUserById(int userId);
        Task<int> ChangePassword(ChangePwd model, int UserId);
        Task<PageFunctionProcedure> GetPageFunctions(int UserId);
        void Dispose();
    }
}
