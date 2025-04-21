using SchoolCoreWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolCoreWEB.IServices
{
    public interface ICallService
    {
        //Task<HttpResponseMessage> GetApi(string api, string token, string userid, string sessionyearid);
        Task<HttpResponseMessage> LoginApi(string jsonInput, string api);
        Task<HttpResponseMessage> GetApiUser(string api, string token, string userid);
        Task<HttpResponseMessage> GetApi(string api, string token, string param);      
       
        Task<HttpResponseMessage> PostApi(string jsonInput, string api, string token, string param);
        Task<HttpResponseMessage> PostFileApi(List<IFormFile> files, string objectJson, string api, string token, string param);     
        Task<HttpResponseMessage> PutApi(string jsonInput, string api, string token, string param);
        Task<HttpResponseMessage> DeleteApi(string api, string token, string param);
        Task<T> FetchObjectFromResponse<T>(HttpResponseMessage result);
        Task<List<T>> FetchObjectListFromResponse<T>(HttpResponseMessage result);
        Task<string> FetchStringFromResponse(HttpResponseMessage result);
        string FetchApiUrl();
       
    }
}
