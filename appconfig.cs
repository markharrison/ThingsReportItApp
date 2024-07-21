using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Azure.Storage.Blobs;

namespace ThingsReportIt
{
    public class AppConfig
    {
        public BlobContainerClient _BCclient { get; set; }
        public string? _EventGridTopicEndpoint { get; set; }
        public string? _EventGridKey { get; set; }
        public string? _ThingsStorageConnectionString { get; set; }
        public string? _ImagesContainer { get; set; }
        public string? _AdminPW { get; set; }

        public AppConfig(IConfiguration config)
        {
            _EventGridTopicEndpoint = config.GetValue<string>("EventGridTopicEndpoint");
            if (_EventGridTopicEndpoint == null || _EventGridTopicEndpoint == "")
            {
                var msg = "EventGridTopicEndpoint is not configured";
                Console.WriteLine(msg);
                throw new Exception(msg);
            }

            _EventGridKey = config.GetValue<string>("EventGridKey");
            if (_EventGridKey == null || _EventGridKey == "")
            {
                var msg = "EventGridKey is not configured";
                Console.WriteLine(msg);
                throw new Exception(msg);
            }

            _ThingsStorageConnectionString = config.GetConnectionString("ThingsStorageConnectionString");
            if (_ThingsStorageConnectionString == null || _ThingsStorageConnectionString == "")
            {
                var msg = "ThingsStorageConnectionString is not configured";
                Console.WriteLine(msg);
                throw new Exception(msg);
            }

            _ImagesContainer = config.GetValue<string>("ImagesContainer");
            if (_ImagesContainer == null || _ImagesContainer == "")
            {
                _ImagesContainer = "images";
            }

            _AdminPW = config.GetValue<string>("AdminPW");

            try
            {
                _BCclient = new BlobContainerClient(_ThingsStorageConnectionString, _ImagesContainer);
                _BCclient.CreateIfNotExists(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw;
            }

        }

    }

}
