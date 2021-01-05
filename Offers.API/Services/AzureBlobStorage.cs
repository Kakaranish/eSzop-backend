﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Offers.API.Services
{
    public class AzureBlobStorage : IBlobStorage
    {
        private readonly AzureStorageConfig _azureConfig;
        private readonly BlobContainerClient _containerClient;
        private bool _ensured = false;

        public AzureBlobStorage(IOptions<AzureStorageConfig> azureOptions)
        {
            _azureConfig = azureOptions?.Value ?? throw new ArgumentNullException(nameof(azureOptions.Value));
            _containerClient = new BlobContainerClient(_azureConfig.ConnectionString, _azureConfig.ContainerName);
        }

        public string ContainerName => _azureConfig.ContainerName;

        public async Task UploadAsync(Stream content, string blobName)
        {
            await EnsureContainerExistsAsync();

            var blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(content);
        }

        public async Task<Stream> DownloadAsync(string blobName)
        {
            await EnsureContainerExistsAsync();

            var blobClient = _containerClient.GetBlobClient(blobName);
            var downloadInfo = await blobClient.DownloadAsync();

            return downloadInfo.Value.Content;
        }

        private async Task EnsureContainerExistsAsync()
        {
            if (_ensured) return;

            await _containerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
            _ensured = true;
        }
    }
}
