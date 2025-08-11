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
        public static List<Subject> Subjects { get; set; } = new();
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
                return View("UserGallery", (CurrentUser.Images, Subjects));
            CurrentUser = new(Request.Form["Username"], Request.Form["Password"], new List<TheImage>());
            IsLoggedIn = true;
            return View("UserGallery", (CurrentUser.Images, Subjects));
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
            string? subject = Request.Form["chosenSubject"];
            TheImage? newImage = new(name, password, image, CurrentUser, creationTime,new Subject(subject));
            CurrentUser.Images.Add(newImage);
            CurrentUser.Images[CurrentUser.Images.Count - 1].Id = CurrentUser.Images.Count - 1;
            return View("UserGallery", (CurrentUser.Images, Subjects));
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
            return View("UserGallery", (CurrentUser.Images, Subjects));
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
            int? id = Convert.ToInt32(Request.Form["imageIdDeleteImage"]);
            if (CurrentUser.Images[id.Value].Password.Equals(password))
                CurrentUser.Images.Remove(CurrentUser.Images[id.Value]);
            return View("UserGallery", (CurrentUser.Images, Subjects));
        }
        [HttpPost("DuplicateImage")]
        public IActionResult DuplicateImage()
        {
            int? id = Convert.ToInt32(Request.Form["imageId"]);
            CurrentUser.Images.Add(new TheImage(CurrentUser.Images[id.Value].Name, CurrentUser.Images[id.Value].Password, 
                CurrentUser.Images[id.Value].Image, CurrentUser, DateTime.Now, CurrentUser.Images[id.Value].Subject));
            return View("UserGallery", (CurrentUser.Images,Subjects));
        }
        [HttpPost("AddSubject")]
        public IActionResult AddSubject()
        {
            string? subject = Request.Form["subjectName"];
            Subjects.Add(new Subject(subject));
            return View("UserGallery",(CurrentUser.Images,Subjects));
        }
    }
}
