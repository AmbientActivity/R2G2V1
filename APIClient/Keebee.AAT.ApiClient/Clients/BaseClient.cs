using System;
using System.Net;
using System.Security;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public class BaseClient
    {
        private readonly RestClient _client = new RestClient("http://localhost/Keebee.AAT.Operations/api/")
        {
            Timeout = 120000
        };

        public IRestResponse Execute(RestRequest request)
        {
            var response = _client.Execute(request);
            HandleHttpStatusCode(response);
            return response;
        }

        public static void HandleHttpStatusCode(IRestResponse response)
        {
            var exception = ErrorException(response);
            if (exception != null)
                throw exception;
        }

        protected static Exception ErrorException(IRestResponse response)
        {
            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details.";
                return new ApplicationException(message, response.ErrorException);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new SecurityException();

            if (response.StatusCode == HttpStatusCode.Forbidden)
                return new SecurityException();

            return null;
        }
    }
}
