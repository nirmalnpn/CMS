using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Shared.Services
{
    public static class Services
    {
        public static HttpClient Initial()
        {
            var Client = new HttpClient();  
            Client.Timeout = TimeSpan.FromHours(1);
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", url.token == null ? "n/a" : url.token);
            Client.BaseAddress = new Uri(url.Baseurl.ToString());
            return Client;
        }
    }
    public static class url
    {
        public static string Baseurl { get; set; }
        public static string token { get; set; } = string.Empty;
    }
}
