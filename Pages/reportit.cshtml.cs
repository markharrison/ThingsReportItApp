using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ThingsReportIt.Pages
{
    public class ReportItModel : PageModel
    {
        public string? responseTitle;
        public int responseStatusCode;

        private readonly BlobContainerClient _BCclient;
        private readonly string _EventGridTopicEndpoint;
        private readonly string _EventGridKey;
        private readonly IHttpClientFactory _httpClientFactory;


        public ReportItModel(AppConfig appconfig, IHttpClientFactory httpClientFactory)
        {
            _BCclient = appconfig._BCclient;
            _EventGridTopicEndpoint = appconfig._EventGridTopicEndpoint ?? string.Empty;
            _EventGridKey = appconfig._EventGridKey ?? string.Empty;
            _httpClientFactory = httpClientFactory;
        }

        public void OnGet()
        {
            responseTitle = "Client Error";
            responseStatusCode = StatusCodes.Status422UnprocessableEntity;
        }

        public class JsonContent : StringContent
        {
            public JsonContent(object obj) :
                base(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json")
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

            if (RIData.Image == null || RIData.Name == null)
            {
                responseTitle = "Bad Request - Image data is missing";
                responseStatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            BlobClient? _Bclient = null;

            try
            {
                byte[] photodata = Convert.FromBase64String(RIData.Image.Replace("data:image/jpeg;base64,", ""));
                _Bclient = _BCclient.GetBlobClient("image-" + GetTimestamp() + "-" + RemoveNonAlphanumericChars(RIData.Name.Trim().ToLower()) + ".jpg");
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
                ThingItem TIData = new ThingItem
                {
                    Thingid = RIData.Thingid,
                    Name = RIData.Name,
                    Latitude = RIData.Latitude,
                    Longitude = RIData.Longitude,
                    Image = _Bclient.Uri.ToString(),
                    Text = RIData.Text,
                    Status = "?",
                    Data = ""
                };

                EventGridPostData EGData = new EventGridPostData
                {
                    subject = "Alarm",
                    id = Guid.NewGuid().ToString(),
                    eventType = "AlarmTrigger",
                    eventTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFK"),
                    data = TIData
                };

                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("aeg-sas-key", _EventGridKey);

                var jsonContent = new JsonContent(new[] { EGData });
                HttpResponseMessage response = await httpClient.PostAsync(_EventGridTopicEndpoint, jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    responseTitle = "Internal Server Error - Response from EventGrid: " + response.StatusCode;
                    responseStatusCode = StatusCodes.Status500InternalServerError;
                    return;
                }

            }
            catch (Exception ex)
            {
                responseTitle = "Internal Server Error - " + ex.Message;
                responseStatusCode = StatusCodes.Status500InternalServerError;
                return;
            }

            responseTitle = "Success";
            responseStatusCode = StatusCodes.Status200OK;
        }

        public async Task OnPostAsync()
        {
            ReportItPostData? RIData = null;

            try
            {
                var reader = new StreamReader(Request.Body, System.Text.Encoding.UTF8);
                var task = reader.ReadToEndAsync();
                RIData = System.Text.Json.JsonSerializer.Deserialize<ReportItPostData>(await task);


            }
            catch (Exception ex)
            {
                responseTitle = "Internal Server Error - " + ex.Message;
                responseStatusCode = StatusCodes.Status500InternalServerError;
                return;
            }

            if (RIData == null)
            {
                responseTitle = "Bad Request - Missing Report Data";
                responseStatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            await ProcessReport(RIData);

        }
    }

    public class ReportItPostData
    {
        public int Thingid { get; set; }
        public string? Image { get; set; }
        public string? Name { get; set; }
        public string? Text { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class ThingItem
    {
        public long Thingid { get; set; }
        public string? Name { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Image { get; set; }
        public string? Text { get; set; }
        public string? Status { get; set; }
        public string? Data { get; set; }
    }

    public class EventGridPostData
    {
        public required string subject { get; set; }
        public required string id { get; set; }
        public required string eventType { get; set; }
        public required string eventTime { get; set; }
        public required ThingItem data { get; set; }
    }

}

