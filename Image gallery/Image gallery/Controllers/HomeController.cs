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
        public static bool IsLoggedIn { get; set; } = false;
        public HomeController() { }
        [Route("/")]
        public IActionResult Login()
        {
            return View("Login");
        }
        [HttpPost("UserGallery")]
        public IActionResult UserGallery()
        {
            if (IsLoggedIn)
                return View("UserGallery", CurrentUser.Images);
            CurrentUser = new(Request.Form["Username"], Request.Form["Password"], new List<TheImage>());
            IsLoggedIn = true;
            return View("UserGallery", CurrentUser.Images);
        }
        public IActionResult LogOff()
        {
            CurrentUser = null;
            return Login();
        }
        public static async Task<byte[]?> ConvertImageToBytes(IFormFile file)
        {
            if (file is not null && file.Length > 0)
            {
                using (MemoryStream stream = new())
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
            string? name = Request.Form["imageName"];
            DateTime creationTime = DateTime.Now;
            byte[]? image = ConvertImageToBytes(Request.Form.Files["image"]).Result;
            string? password = Request.Form["ImagePassword"];
            TheImage? newImage = new(name, password, image, CurrentUser, creationTime);
            CurrentUser.Images.Add(newImage);
            return View("UserGallery", CurrentUser.Images);
        }
        [HttpPost("UpdateImage")]
        public IActionResult UpdateImage()
        {
            int? id = Convert.ToInt32(Request.Form["imageId"]);
            string? name = Request.Form["imageName"];
            DateTime creationTime = DateTime.Now;
            byte[]? image = ConvertImageToBytes(Request.Form.Files["image"]).Result;
            string? password = Request.Form["ImagePassword"];
            CurrentUser.Images[id.Value] = new TheImage(name, password, image, CurrentUser, creationTime);
            return View("UserGallery", CurrentUser.Images);
        }
        [HttpPost("DeleteImage")]
        public IActionResult DeleteImage()
        {
            string? password = "";
            try
            {
                password = Request.Form["imagePassword"];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            int? id = Convert.ToInt32(Request.Form["imageId"]);
            if (CurrentUser.Images[id.Value].Password.Equals(password))
                CurrentUser.Images.Remove(CurrentUser.Images[id.Value]);
            return View("UserGallery", CurrentUser.Images);
        }
        [HttpPost("DuplicateImage")]
        public IActionResult DuplicateImage()
        {
            int? id = Convert.ToInt32(Request.Form["imageId"]);
            CurrentUser.Images.Add(new TheImage(CurrentUser.Name, CurrentUser.Password, CurrentUser.Images[id.Value].Image, CurrentUser, DateTime.Now));
            return View("UserGallery", CurrentUser.Images);
        }
    }
}
