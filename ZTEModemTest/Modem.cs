using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ZTEModemTest
{
    public class Modem : IModem, IModemSendSms, IModemLogout
    {
        private readonly string modemIP;
        private CookieContainer cookieContainer;
        private readonly Uri setUri;
        private readonly Uri getUri;
        private readonly Uri referrer;
        private HttpClient httpClient;
        private HttpClientHandler handler;
        public Modem()
        {
            cookieContainer = new CookieContainer();
            modemIP = ConfigurationManager.AppSettings["modemIP"];
            setUri = new Uri($"http://{modemIP}/goform/goform_set_cmd_process");
            getUri = new Uri($"http://{modemIP}/goform/goform_get_cmd_process");
            referrer = new Uri($"http://{modemIP}/index.html");
            handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            handler.UseCookies = true;
            httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Referrer = referrer;
        }

        public void Logout()
        {
            var data = new Dictionary<string, string>
            {
                {"isTest", "false"},
                {"goformId", "LOGOUT"},
                {"AD", CalculateAD()}
            };
            var res = httpClient.PostAsync(setUri, new FormUrlEncodedContent(data)).Result;

            var content = res.Content.ReadAsStringAsync();
            JsonSerializer.Deserialize<LoginResult>(content.Result);
        }
        public  IModemSendSms Login()
        {
            var encodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["Password"]));
            var data = new Dictionary<string, string>
            {
                {"isTest", "false"},
                {"goformId", "LOGIN"},
                {"password", encodedPassword}
            };
            var res = httpClient.PostAsync(setUri, new FormUrlEncodedContent(data)).Result;

            var content = res.Content.ReadAsStringAsync();
            var status = JsonSerializer.Deserialize<LoginResult>(content.Result);
            return status.result == "0" ? this : null;
        }
        public  IModemLogout SendSms(string phoneNumber, string message)
        {
            var data = new Dictionary<string, string>
            {
                {"isTest", "false"},
                {"goformId", "SEND_SMS"},
                {"notCallback", "true"},
                {"Number", phoneNumber},
                {"sms_time", DateTime.UtcNow.ToString("y;m;d;H;i;s;+4.5")},
                {"MessageBody", message.UnicodeStr2HexStr()},
                {"ID", "-1"},
                {"encode_type", "UNICODE"},
                {"AD", CalculateAD()}
            };
            var res = httpClient.PostAsync(setUri, new FormUrlEncodedContent(data)).Result;

            var content = res.Content.ReadAsStringAsync();
            var statue = JsonSerializer.Deserialize<LoginResult>(content.Result);
            return statue.result == "success" ? this : null;
        }
        private ModemVersion GetModemVersion()
        {
            var builder = new UriBuilder(getUri)
            {
                Query = "isTest=false&cmd=Language,cr_version,wa_inner_version&multi_data=1"
            };
            var url = builder.Uri.AbsoluteUri;
            var res = httpClient.GetAsync(url).Result;
            var content = res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ModemVersion>(content.Result);
        }
        private RDNumber GetRDString()
        {
            // GET     http://192.168.0.1/goform/goform_get_cmd_process?isTest=false&cmd=RD&_=1651054019364
            var builder = new UriBuilder(getUri)
            {
                Query = "isTest=false&cmd=RD"
            };
            var url = builder.Uri.AbsoluteUri;
            var res = httpClient.GetAsync(url).Result;
            var content = res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RDNumber>(content.Result);

        }
        private string CalculateAD()
        {
            var modemVersion = GetModemVersion();
            var MD5_RD = modemVersion.GroupRD.CreateMD5().ToLower();
            var info_RD = GetRDString();
            var rd_mix = MD5_RD + info_RD.RD;
            var MD5_AD = rd_mix.CreateMD5();
            return MD5_AD.ToLower();
        }
        private string RetriveCookie(string cookieName)
        {
            var cookie = cookieContainer.GetCookies(setUri).Cast<Cookie>().FirstOrDefault(x => x.Name == cookieName);
            return cookie?.Value;
        }
        private void RetriveAllCookies()
        {
            IEnumerable<Cookie> responseCookies = cookieContainer.GetCookies(setUri).Cast<Cookie>();
            foreach (Cookie _cookie in responseCookies)
            {
                Console.WriteLine(_cookie.Name + ": " + _cookie.Value);
            }
        }
    }
}
