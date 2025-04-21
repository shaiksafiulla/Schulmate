using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IReferenceAdmissionService
    {       
        List<VReferenceAdmission> GetAllReferenceAdmission();
        VReferenceAdmission ViewReferenceAdmission(int Id);
        ReferenceAdmission GetReferenceAdmission(int Id);
        ServiceResult SaveReferenceAdmission(ReferenceAdmission model, int UserId);
        ServiceResult DeleteReferenceAdmission(int Id, int UserId);
        bool IsRecordExists(string name, int Id);
        void Dispose();

    }
}
