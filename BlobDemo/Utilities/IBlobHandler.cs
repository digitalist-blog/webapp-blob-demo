using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlobDemo.Models;

namespace BlobDemo.Utilities
{
  public interface IBlobHandler
  {
    Task UploadFileAsync(string FileName, Stream stream);
    Task<List<BlobModel>> GetFileList();
    Stream DownloadFile(string FileName);
  }
}