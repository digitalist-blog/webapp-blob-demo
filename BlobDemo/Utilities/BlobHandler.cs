
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlobDemo.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BlobDemo.Utilities
{
  public class BlobHandler : IBlobHandler
  {
    const string CONNECTION_STRING_CONFIG_NAME = "BLOB_CONN_STRING";
    const string BLOB_CONTAINER_NAME = "BLOB_CONTAINER_NAME";

    string connectionString = string.Empty;
    string containerName = string.Empty;
    ILogger _logger;

    public BlobHandler(IConfiguration configuration, ILogger<BlobHandler> logger)
    {
      connectionString = configuration.GetConnectionString(CONNECTION_STRING_CONFIG_NAME);
      containerName = configuration.GetValue<string>(BLOB_CONTAINER_NAME);
      _logger = logger;
    }

    public Stream DownloadFile(string FileName)
    {
      _logger.LogInformation("Staring to download file");
      BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
      BlobClient blob = container.GetBlobClient(FileName);

      if (blob.Exists())
      {
        return blob.OpenRead();
      }

      _logger.LogError("{0} file not found!", FileName);
      throw new FileNotFoundException(FileName);
    }

    public async Task<List<BlobModel>> GetFileList()
    {
      _logger.LogInformation("Starting Listing of blobs...");
      BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
      var resultSegment = container.GetBlobsAsync().AsPages(default, 100);

      List<BlobModel> list = new List<BlobModel>();

      await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
      {
        foreach (BlobItem blobItem in blobPage.Values)
        {
          _logger.LogInformation("Blob name: {0}", blobItem.Name);
          list.Add(new BlobModel 
                       { 
                         BlobName = blobItem.Name, 
                         Size = blobItem.Properties.ContentLength.GetValueOrDefault(0) 
                       });
        }

      }
      return list;
    }

    public async Task UploadFileAsync(string FileName, Stream stream)
    {
      _logger.LogInformation("Beginning upload for {0}", FileName);
      BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
      BlobClient blob = container.GetBlobClient(FileName);
      var info = await blob.UploadAsync(stream);

      _logger.LogInformation(info.Value.ETag.ToString());
    }
  }
}