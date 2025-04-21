using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;
using System.Data;
using System.Net.Http.Headers;
using System.Reflection;

namespace SchoolCoreWEB.Services
{
    public class CallService : ICallService
    {
        private readonly IConfiguration _config;

        public CallService(IConfiguration config)
        {
            _config = config;
        }
        public string FetchApiUrl()
        {
            return _config["WebApi:ImageBaseUrl"];
        }
        public async Task<HttpResponseMessage> LoginApi(string jsonInput, string api)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_config["WebApi:BaseUrl"]);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Add String Content
                    var stringContent = new StringContent(jsonInput);
                    // Assign Content Type Header
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    // Make an API call and receive HttpResponseMessage
                    HttpResponseMessage responseMessage;
                    responseMessage = await client.PostAsync(api, stringContent);
                    return responseMessage;
                }
                // Create HttpClient

            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public async Task<HttpResponseMessage> GetApiUser(string api, string token, string userid)
        {
            try
            {
                // Create HttpClient
                var client = new HttpClient { BaseAddress = new Uri(_config["WebApi:BaseUrl"]) };
                client.DefaultRequestHeaders.Accept.Clear();

                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                client.DefaultRequestHeaders.Add("UserId", userid);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //client.DefaultRequestHeaders.Add("SessionYearId", sessionyearid);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage responseMessage;
                responseMessage = await client.GetAsync(api);
                return responseMessage;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
       
        public async Task<HttpResponseMessage> GetApi(string api, string token, string param)
        {
            try
            {
                // Create HttpClient
                var client = new HttpClient { BaseAddress = new Uri(_config["WebApi:BaseUrl"]) };
                client.DefaultRequestHeaders.Accept.Clear();

                //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                //client.DefaultRequestHeaders.Add("UserId", userid);                
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //client.DefaultRequestHeaders.Add("SessionYearId", sessionyearid);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Add("UserParam", param);

                HttpResponseMessage responseMessage;
                responseMessage = await client.GetAsync(api);
                return responseMessage;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public async Task<HttpResponseMessage> PostApi(string jsonInput, string api, string token, string param)
        {
            try
            {
                using (var client = new HttpClient())
                {                 
                    client.BaseAddress = new Uri(_config["WebApi:BaseUrl"]);
                    client.DefaultRequestHeaders.Accept.Clear();

                    //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    //client.DefaultRequestHeaders.Add("UserId", userid);
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //client.DefaultRequestHeaders.Add("SessionYearId", sessionyearid);
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("UserParam", param);

                    
                    var stringContent = new StringContent(jsonInput);                 
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                   
                    HttpResponseMessage responseMessage;
                    responseMessage = await client.PostAsync(api, stringContent);
                    return responseMessage;
                }               

            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public async Task<HttpResponseMessage> PostFileApi(List<IFormFile> files, string objectJson, string api, string token, string param)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //var client = new HttpClient { BaseAddress = new Uri(host) };
                    client.BaseAddress = new Uri(_config["WebApi:BaseUrl"]);
                    client.DefaultRequestHeaders.Accept.Clear();

                    //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    //client.DefaultRequestHeaders.Add("UserId", userid);                 

                    //client.DefaultRequestHeaders.Add("SessionYearId", sessionyearid);
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("UserParam", param);

                    using var formData = new MultipartFormDataContent();
                  
                    var strContent = new StringContent(objectJson);
                    strContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    formData.Add(strContent, "object");

                    if (files.Count > 0)
                    {
                        //HttpPostedFileBase
                        for (int i = 0; i < files.Count; i++)
                        {
                            // Add the file to the content
                            //byte[] fileBytes = new byte[files[i].InputStream.Length];
                            //files[i].InputStream.Read(fileBytes, 0, fileBytes.Length);
                            if(files[i] != null)
                            {
                                var streamContent = new StreamContent(files[i].OpenReadStream());
                                streamContent.Headers.Add("Content-Type", files[i].ContentType);
                                formData.Add(streamContent, "file", files[i].FileName);
                            }                    
                        }
                    }
                    // Make the request to the Web API
                    var response = client.PostAsync(api, formData).Result;
                    return response;
                }
                // Create HttpClient
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<HttpResponseMessage> PutApi(string jsonInput, string api, string token, string param)
        {
            try
            {
                using (var client = new HttpClient())
                {                  
                    client.BaseAddress = new Uri(_config["WebApi:BaseUrl"]);
                    client.DefaultRequestHeaders.Accept.Clear();

                    //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");                    
                    //client.DefaultRequestHeaders.Add("UserId", userid);
                    //client.DefaultRequestHeaders.Add("SessionYearId", sessionyearid);
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("UserParam", param);
                
                    var stringContent = new StringContent(jsonInput);                 
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                 
                    HttpResponseMessage responseMessage;
                    responseMessage = await client.PutAsync(api, stringContent);
                    return responseMessage;
                }              

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<HttpResponseMessage> DeleteApi(string api, string token, string param)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_config["WebApi:BaseUrl"]);
                    client.DefaultRequestHeaders.Accept.Clear();

                    //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");                            
                    //client.DefaultRequestHeaders.Add("UserId", userid);
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //client.DefaultRequestHeaders.Add("SessionYearId", sessionyearid);
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("UserParam", param);

                    HttpResponseMessage responseMessage;
                    responseMessage = await client.DeleteAsync(api);
                    return responseMessage;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<T> FetchObjectFromResponse<T>(HttpResponseMessage result)
        {
            try
            {
                // Convert the HttpResponseMessage to Byte[]
                var resultArray = await result.Content.ReadAsStringAsync();
                // Deserialize the Json string into type using JsonConvert
                var final = JsonConvert.DeserializeObject<T>(resultArray);

                return final;
            }
            catch (Exception ex)
            {
                // throw ex;
                return default;
            }
        }
        public async Task<List<T>> FetchObjectListFromResponse<T>(HttpResponseMessage result)
        {
            try
            {
                // Convert the HttpResponseMessage to Byte[]
                var resultArray = await result.Content.ReadAsStringAsync();
                // Deserialize the Json string into type using JsonConvert
                var final = JsonConvert.DeserializeObject<List<T>>(resultArray);
                return final;
            }
            catch (Exception ex)
            {
                return default;
            }
        }
        public async Task<string> FetchStringFromResponse(HttpResponseMessage result)
        {
            try
            {
                // Convert the HttpResponseMessage to Byte[]
                var data = await result.Content.ReadAsStringAsync();
                return data;
            }
            catch (Exception ex)
            {
                return default;
            }
        }
    }
}