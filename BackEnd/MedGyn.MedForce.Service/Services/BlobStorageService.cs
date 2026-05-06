using System;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Service.Interfaces;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Options;

namespace MedGyn.MedForce.Service.Services
{
	public class BlobStorageService : IBlobStorageService
	{
		string accessKey = string.Empty;
		string containerName = string.Empty;

		public BlobStorageService(
			IOptions<ConnectionStrings> connectionStrings,
			IOptions<AppSettings> appSettings
		)
		{
			accessKey     = connectionStrings.Value.AccessKey;
			containerName = appSettings.Value.AzureBlobContainer;
		}

		public async Task<string> UploadFileToBlob(string fileName, string base64String) {
			if(string.IsNullOrEmpty(fileName.Trim()) || string.IsNullOrEmpty(base64String.Trim())) {
				return null;
			}

			if(CloudStorageAccount.TryParse(accessKey, out CloudStorageAccount storageAccount))
			{
				var blobClient = storageAccount.CreateCloudBlobClient();
				var container = blobClient.GetContainerReference(containerName.ToLower()); // container must be lower case

				// base64String = "data:<fileType>;base64,<data>"
				var split = base64String.Split(';');
				var fileType = split[0].Substring("data:".Length);
				var data = split[1].Substring("base64,".Length); // can't convert the "base64,"

				var blob = container.GetBlockBlobReference(fileName);
				blob.Properties.ContentType = fileType;
				var bytes = Convert.FromBase64String(data);
				await blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);

				return blob.Uri.ToString();
			}

			return null;
		}
	}
}
