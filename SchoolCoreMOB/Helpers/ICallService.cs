namespace SchoolCoreMOB.IServices
{
    public interface ICallService
    {
        Task<HttpResponseMessage> LoginApi(string jsonInput, string api);
        Task<HttpResponseMessage> WebAuthnApi(string jsonInput, string api);
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
