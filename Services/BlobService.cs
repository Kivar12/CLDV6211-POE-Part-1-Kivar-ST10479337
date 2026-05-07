using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace EventEaseAssignment.Services
{
    public class BlobService
    {
        private readonly string _connectionString;

        public BlobService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("AzureStorage")!;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var containerName = "venue-images";

            var containerClient = new BlobContainerClient(_connectionString, containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            var blobClient = containerClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return blobClient.Uri.ToString();
        }
    }
}