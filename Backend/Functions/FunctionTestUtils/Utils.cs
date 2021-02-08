using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;

namespace Sanet.SmartSkating.Backend.Functions.TestUtils
{
    public static class Utils
    {
        public static HttpRequest CreateMockRequest(object body = null, string queryString = null)
        {
            var mockRequest = new DefaultHttpContext().Request;

            if (body != null)
            {
                var ms = new MemoryStream();
                var sw = new StreamWriter(ms);

                var json = JsonConvert.SerializeObject(body);

                sw.Write(json);
                sw.Flush();

                ms.Position = 0;
                mockRequest.Body = ms;
            }

            if (!string.IsNullOrEmpty(queryString))
            {
                mockRequest.QueryString = new QueryString(queryString);
            }

            return mockRequest;
        }
    }
}
