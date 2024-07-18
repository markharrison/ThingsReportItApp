using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace ThingsReportIt.Pages
{
    public class AppConfigInfoModel : PageModel
    {
        IConfiguration _config;
        AppConfig _appconfig;
        public string strHtml;

        public AppConfigInfoModel(IConfiguration config, AppConfig appconfig)
        {
            _config = config;
            _appconfig = appconfig;
            strHtml = "";
        }

        public void OnGet()
        {
            string EchoData(string key, string value)
            {
                return key + ": <span style='color: blue'>" + value + "</span><br/>";
            }

            string obj2string(object obj)
            {
                return obj?.ToString() ?? "";
            }

            strHtml += EchoData("OS Description", System.Runtime.InteropServices.RuntimeInformation.OSDescription);
            strHtml += EchoData("Framework Description", System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
            strHtml += EchoData("BuildIdentifier", _config.GetValue<string>("BuildIdentifier") ?? "");

            if (_appconfig._AdminPW == HttpContext.Request.Query["pw"].ToString())
            {
                strHtml += EchoData("ASPNETCORE_ENVIRONMENT", _config.GetValue<string>("ASPNETCORE_ENVIRONMENT") ?? "");
                strHtml += EchoData("APPLICATIONINSIGHTS_CONNECTION_STRING", _config.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING") ?? "");
                strHtml += EchoData("LogicAppEndpoint", _appconfig._LogicAppEndpoint ?? "");
                strHtml += EchoData("ThingsStorageConnectionString", _appconfig._ThingsStorageConnectionString ?? "");
                strHtml += EchoData("ImagesContainer", _appconfig._ImagesContainer ?? "");
                strHtml += EchoData("CameraSelection", obj2string(Request.Cookies["CameraSelection"] ?? ""));
                strHtml += EchoData("CameraScreenSize", obj2string(Request.Cookies["CameraScreenSize"] ?? ""));
                strHtml += EchoData("JPGQuality", obj2string(Request.Cookies["JPGQuality"] ?? ""));
                strHtml += EchoData("Thingid", obj2string(Request.Cookies["Thingid"] ?? ""));
            }
        }
    }
}