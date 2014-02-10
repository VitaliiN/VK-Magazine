using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKMagazine.Response;
using RestSharp;


namespace VKMagazine.RequestWrapper
{

    public static class VkRequestWrapper
    {
        private static string baseUrl = "https://api.vk.com/method/";
        public static void DoRequest<T>(string methodName, Dictionary<string, string> parameters, Action<IRestResponse<T>,RestRequestAsyncHandle> callback) where T : new()
        {
            RestClient client = new RestClient(baseUrl+methodName+"/");
            RestRequest request = new RestRequest();
            foreach (var prmtr in parameters)
            {
                request.AddParameter(prmtr.Key, prmtr.Value);
            }
            client.ExecuteAsync<T>(request, callback);
           
        }

        public static void DoRequest(string methodName, Dictionary<string, string> parameters, Action<IRestResponse, RestRequestAsyncHandle> callback) 
        {
            RestClient client = new RestClient(baseUrl + methodName + "/");
            RestRequest request = new RestRequest();
            foreach (var prmtr in parameters)
            {
                request.AddParameter(prmtr.Key, prmtr.Value);
            }
            client.ExecuteAsync(request, callback);

        }
    }
}
