
using System.Collections.Generic;

namespace BlobDemo.Models
{
  public class BlobModel
  {
    public string BlobName { get; set; }
    public long Size { get; set; }
  }

  public class BlobModelCollection
  {
    public List<BlobModel> Blobs { get; set; }
  }
}