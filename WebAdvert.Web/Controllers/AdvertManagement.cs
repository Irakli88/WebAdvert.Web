using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.AdvertManagement;
using WebAdvert.Web.Services;

namespace WebAdvert.Web.Controllers
{
    public class AdvertManagement : Controller
    {
        public readonly IFileUploader _fileUploader;

        public AdvertManagement(IFileUploader fileUploader)
        {
            _fileUploader = fileUploader;
        }
        public IActionResult Create(CreateAdvertViewModel model)
        {
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateAdvertViewModel model, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var id = "1111";  //this is the ID of advertisment saved to DB prior to this call

                var fileName = "";
                if (imageFile != null)
                {
                    fileName = !string.IsNullOrWhiteSpace(imageFile.FileName) ? Path.GetFileName(imageFile.FileName) : id;
                    var filePath = $"{id}/{fileName}";

                    try
                    {
                        using (var readStream = imageFile.OpenReadStream())
                        {
                            var result = await _fileUploader.UploadFileAsync(filePath, readStream);
                            if (!result)
                                throw new Exception("Could not upload the image to the file repository. Please see the logs.");
                        }

                        //call advert API and confirm advertisement

                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception ex)
                    {
                        //call advert API and cancel the advertisement
                        Console.WriteLine(ex);
                    }
                }
            }
            return View(model);
        }
    }
}
