using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Azure.Storage.Blobs;
//using Microsoft.Azure.Storage;
//using Microsoft.Azure.Storage.Blob;

namespace ThingsReportIt
{
    public class AppConfig
    {
        public BlobContainerClient _BCclient { get; set; }
        public string _LogicAppEndpoint { get; set; }

        public string _ThingsStorageConnectionString { get; set; }

        public string _ImagesContainer { get; set; }

        public string _AdminPW;

        public AppConfig(IConfiguration config)
        {
            _LogicAppEndpoint = config.GetValue<string>("LogicAppEndpoint");
            if (_LogicAppEndpoint == null || _LogicAppEndpoint == "")
            {
                Debug.WriteLine("LogicAppEndpoint is not configured");
            }

            _ThingsStorageConnectionString = config.GetConnectionString("ThingsStorageConnectionString");
            if (_ThingsStorageConnectionString == null || _ThingsStorageConnectionString == "")
            {
                Debug.WriteLine("ThingsStorageConnectionString is not configured");
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

                //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(vCS);
                //CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                //imagesContainer = cloudBlobClient.GetContainerReference(strContainerName);
                //imagesContainer.CreateIfNotExists();
                //BlobContainerPermissions permissions = new BlobContainerPermissions
                //{
                //    PublicAccess = BlobContainerPublicAccessType.Blob
                //};
                //imagesContainer.SetPermissions(permissions);

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw;
            }

        }

    }

}
