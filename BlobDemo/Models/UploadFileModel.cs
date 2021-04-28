
using Microsoft.AspNetCore.Http;

namespace BlobDemo.Models
{
  public class UploadfileModel
  {
    public IFormFile file { get; set; }
  }
}