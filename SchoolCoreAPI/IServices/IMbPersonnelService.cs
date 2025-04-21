using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IMbPersonnelService
    {       
        Task<MbPersonnelDetail> GetPersonnelDetail(int Id, int SessionYearId, string PersonnelType);
        void Dispose();

    }
}
