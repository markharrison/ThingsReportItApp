using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Net.Http;
using Azure.Storage.Blobs;

namespace ThingsReportIt.Pages
{
    public class ReportItModel : PageModel
    {
        public string responseTitle;
        public int responseStatusCode;

        private readonly BlobContainerClient _BCclient;

        private readonly string _LogicAppEndpoint;


        public ReportItModel(AppConfig _appconfig)
        {
            this._BCclient = _appconfig._BCclient;
            this._LogicAppEndpoint = _appconfig._LogicAppEndpoint;
        }

        public void OnGet()
        {
            responseTitle = "Client Error";
            responseStatusCode = StatusCodes.Status422UnprocessableEntity;
        }

        public class JsonContent : StringContent
        {
            public JsonContent(object obj) :
                base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
            { }
        }

        private async Task ProcessReport(ReportItPostData RIData)
        {
            string RemoveNonAlphanumericChars(string s)
            {
                if (string.IsNullOrEmpty(s))
                    return "null";

                StringBuilder sb = new StringBuilder();
                foreach (var c in s)
                {
                    if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
                        sb.Append(c);;
                }
                return sb.ToString();
            }
            string GetTimestamp()
            {
                DateTime value = DateTime.Now;
                return value.ToString("yyyyMMddHHmmssffff");
            }

            BlobClient _Bclient = null;

            try
            {
                byte[] photodata = Convert.FromBase64String(RIData.image.Replace("data:image/jpeg;base64,", ""));
                _Bclient = _BCclient.GetBlobClient("image-" + GetTimestamp() + "-" + RemoveNonAlphanumericChars(RIData.name.Trim().ToLower()) + "-" + ".jpg");
                _Bclient.DeleteIfExists();

                await _Bclient.UploadAsync(new MemoryStream(photodata));

            }
            catch (Exception ex)
            {
                responseTitle = "Internal Server Error - Storage failure" + ex.Message;
                responseStatusCode = StatusCodes.Status500InternalServerError;
                return;
            }

            try
            {
                LogicAppPostData LAData = new LogicAppPostData
                {
                    thingid = RIData.thingid,
                    image = _Bclient.Uri.ToString(),
                    text = RIData.text,
                    name = RIData.name,
                    latitude = RIData.latitude,
                    longitude = RIData.longitude
                };

                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var jsonToSend = JsonConvert.SerializeObject(LAData, Formatting.None);
                var body = new StringContent(jsonToSend, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(_LogicAppEndpoint, body);
                if (!response.IsSuccessStatusCode)
                {
                    responseTitle = "Internal Server Error - Response from Logic App: " + response.StatusCode;
                    responseStatusCode = StatusCodes.Status500InternalServerError;
                    return;
                }

            }
            catch (Exception ex)
            {

                responseTitle = "Internal Server Error - " + ex.Message;
                responseStatusCode = StatusCodes.Status500InternalServerError;
            }

            responseTitle = "Success";
            responseStatusCode = StatusCodes.Status200OK;
        }

        public async Task OnPostAsync()
        {
            ReportItPostData RIData = null;

            try
            {
                var reader = new StreamReader(Request.Body, System.Text.Encoding.UTF8);
                var task = reader.ReadToEndAsync();
                RIData = JsonConvert.DeserializeObject<ReportItPostData>(task.Result.ToString());

            }
            catch (Exception ex)
            {
                responseTitle = "Internal Server Error - " + ex.Message;
                responseStatusCode = StatusCodes.Status500InternalServerError;
                return;
            }

            await ProcessReport(RIData);

        }
    }

    public class ReportItPostData
    {
        public int thingid { get; set; }
        public string image { get; set; }
        public string name { get; set; }
        public string text { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
    }

    public class LogicAppPostData
    {
        public int thingid { get; set; }
        public string image { get; set; }
        public string name { get; set; }
        public string text { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
    }

}

