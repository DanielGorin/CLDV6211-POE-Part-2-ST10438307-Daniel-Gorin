using Azure.Storage.Blobs;

namespace CLDV6211_POE_Part_1_ST10438307_Daniel_Gorin.Services
{
    public class BlobStorageService
    {
        private readonly string _connectionString;

        public BlobStorageService(IConfiguration config)
        {
            _connectionString = config["AzureStorage:ConnectionString"];
        }
        //CUploads New Blob Storages
        public async Task<string> UploadFileAsync(IFormFile file, string containerName)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync();

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobClient = containerClient.GetBlobClient(uniqueFileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return blobClient.Uri.ToString();
        }
        //Deletes Blob Storages from the container
        public async Task DeleteFileAsync(string blobUrl, string containerName)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Extract the blob name from the URL
            string blobName = Path.GetFileName(new Uri(blobUrl).LocalPath);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}