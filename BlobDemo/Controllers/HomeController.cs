using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlobDemo.Models;
using BlobDemo.Utilities;
using System.IO;

namespace BlobDemo.Controllers
{
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;
    private IBlobHandler _handler;

    public HomeController(ILogger<HomeController> logger, IBlobHandler handler)
    {
      _logger = logger;
      _handler = handler;
    }

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Privacy()
    {
      return View();
    }

    public async Task<IActionResult> ListBlobs()
    {
      BlobModelCollection coll = new BlobModelCollection();
      coll.Blobs = await _handler.GetFileList();

      return View(coll);
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(UploadfileModel model)
    {
      if (model != null)
      {
        if (model.file != null)
        {
          _logger.LogInformation(model.file.FileName);
          var fileName = Path.GetFileName(model.file.FileName);
          await _handler.UploadFileAsync(fileName, model.file.OpenReadStream());
        }
        else
          _logger.LogError("Model file is NULL!");
      }
      else
        _logger.LogError("Model is NULL!");

      return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Download(string itemid)
    {
      string FileName = itemid;
      try
      {
        Stream stream = _handler.DownloadFile(FileName);

        return File(stream, "application/octet-stream", FileName);
      }
      catch(FileNotFoundException)
      {
        return NotFound();
      }
    }
  }
}
