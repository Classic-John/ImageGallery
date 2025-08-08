using System.Collections.Generic;
using Datalayer.Models;
using Datalayer.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Image_gallery.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public static User? CurrentUser { get; set; }
        public HomeController() { }
        [Route("/")]
        public IActionResult Login()
        {
            return View("Login");
        }
        [HttpPost("UserGallery")]
        public IActionResult UserGallery()
        {
            string? name =Request.Form["Username"];
            string? password = Request.Form["Password"];
            return View("UserGallery",new List<TheImage>());
        }
        public static async Task<byte[]?>ConvertImageToBytes(IFormFile file)
        {
            if (file is not null && file.Length > 0)
            {
                using (MemoryStream stream = new() )
                {
                    await file.CopyToAsync(stream);
                    return stream.ToArray();
                }
            }
            return null;
        }
       [HttpPost("AddImage")]
        public IActionResult AddImage()
        {
            string? name=Request.Form["imageName"];
            DateTime creationTime = DateTime.Now;
            byte[]? image = ConvertImageToBytes(Request.Form.Files["image"]).Result;
            string? password = Request.Form["ImagePassword"];
            TheImage? newImage = new(name, password, image, 1, creationTime);
            return View("UserGallery", new List<TheImage>() { newImage } );
        }
    }
}
