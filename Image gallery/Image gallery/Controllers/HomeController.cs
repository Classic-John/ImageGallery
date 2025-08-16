using System.Collections.Generic;
using Core.Services;
using Datalayer.Enums;
using Datalayer.Interfaces;
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
        private readonly IImageFilterService _imageFilterService;
        private readonly ITheImageService _theImageService;
        private readonly IUserService _userService;
        private readonly IEncryptionAndDecryptionService _encryptionAndDecryptionService;
        public HomeController(IImageFilterService imageFilterService, ITheImageService theImageService, IUserService userService, IEncryptionAndDecryptionService encryptionAndDecryptionService)
        {
            _imageFilterService = imageFilterService;
            _theImageService = theImageService;
            _userService = userService;
            _encryptionAndDecryptionService = encryptionAndDecryptionService;
        }
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
            string? name= Request.Form["Username"];
            string? password= Request.Form["Password"];
            CurrentUser = new(name, password, new List<TheImage>());
            _userService.AddUser(CurrentUser);
            IsLoggedIn = true;
            return View("UserGallery", (CurrentUser.Images, Subjects, Enum.GetNames<FilterOption>().ToList()));
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
            SecurityItems items = _encryptionAndDecryptionService.DeriveKeyAndInitializationVectorFromPassword(password);
            byte[] encryptedImage = _encryptionAndDecryptionService.EncryptImage(image, items.Key, items.IV);
            string? hashedPassword = _encryptionAndDecryptionService.HashPassword(password);
            TheImage? newImage = new(name, hashedPassword, encryptedImage, CurrentUser, creationTime, new Subject(subject), items.Key, items.IV,items.Salt);
            CurrentUser.Images.Add(newImage);

            CurrentUser.Images[CurrentUser.Images.Count - 1].Id = CurrentUser.Images.Count - 1;

            return View("UserGallery", (CurrentUser.Images, Subjects, Enum.GetNames<FilterOption>().ToList()));
        }
        [HttpPost("UpdateImage")]
        public IActionResult UpdateImage()
        {
            int? id = Convert.ToInt32(Request.Form["imageId"]);
            string? name = Request.Form["imageName"];
            DateTime creationTime = DateTime.Now;
            string? password = Request.Form["ImagePassword"];
            SecurityItems items = _encryptionAndDecryptionService.DeriveKeyAndInitializationVectorFromPassword(password);
            byte[]? image = _encryptionAndDecryptionService.EncryptImage(ConvertImageToBytes(Request.Form.Files["image"]).Result, items.Key, items.IV);
            string? subject = Request.Form["subjectFromUpdate"];
            string hashedPassword = _encryptionAndDecryptionService.HashPassword(password);
            CurrentUser.Images[id.Value] = new TheImage(name, password, image, CurrentUser, creationTime, new Subject(subject), items.Key, items.IV,items.Salt);

            CurrentUser.Images[CurrentUser.Images.Count - 1].Id = CurrentUser.Images.Count - 1;

            return View("UserGallery", (CurrentUser.Images, Subjects, Enum.GetNames<FilterOption>().ToList()));
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
            if (_encryptionAndDecryptionService.VerifyHashedPassword(password, CurrentUser.Images[id.Value].Password))
                CurrentUser.Images.Remove(CurrentUser.Images[id.Value]);
            return View("UserGallery", (CurrentUser.Images, Subjects, Enum.GetNames<FilterOption>().ToList()));
        }
        [HttpPost("DuplicateImage")]
        public IActionResult DuplicateImage()
        {
            int? id = Convert.ToInt32(Request.Form["imageIdFromDuplication"]);
            string? password = Request.Form["passwordFromDuplication"];
            string? hashedPassword = _encryptionAndDecryptionService.HashPassword(password);
            SecurityItems items = _encryptionAndDecryptionService.DeriveKeyAndInitializationVectorFromPassword(password);
            byte[] image = _encryptionAndDecryptionService.EncryptImage(CurrentUser.Images[id.Value].Image,items.Key,items.IV);
            CurrentUser.Images.Add(new TheImage(CurrentUser.Images[id.Value].Name, hashedPassword,
                image, CurrentUser, DateTime.Now, CurrentUser.Images[id.Value].Subject,
                items.Key, items.IV, items.Salt));

            CurrentUser.Images[CurrentUser.Images.Count - 1].Id = CurrentUser.Images.Count - 1;

            return View("UserGallery", (CurrentUser.Images, Subjects, Enum.GetNames<FilterOption>().ToList()));
        }
        [HttpPost("AddSubject")]
        public IActionResult AddSubject()
        {
            string? subject = Request.Form["subjectName"];
            Subjects.Add(new Subject(subject));
            Subjects[Subjects.Count - 1].Id = Subjects.Count - 1;
            return View("UserGallery", (CurrentUser.Images, Subjects, Enum.GetNames<FilterOption>().ToList()));
        }
        [HttpPost("ApplyImageFilter")]
        public IActionResult ApplyImageFilter()
        {
            int? id = Convert.ToInt32(Request.Form["imageIdFromFilter"]);
            int? chosenFilter = Convert.ToInt32(Request.Form["chosenFilter"]);
            string? password = Request.Form["passwordFromFilterImage"];
            byte[] newImage = _imageFilterService.ApplyChosenFilter(CurrentUser.Images[id.Value].Image, chosenFilter.Value);
            SecurityItems items = _encryptionAndDecryptionService.DeriveKeyAndInitializationVectorFromPassword(password);
            byte[] encryptedImage = _encryptionAndDecryptionService.EncryptImage(newImage, items.Key, items.IV);
            CurrentUser.Images.Add(new(CurrentUser.Images[id.Value].Name, CurrentUser.Images[id.Value].Password, encryptedImage, CurrentUser,
                DateTime.Now, CurrentUser.Images[id.Value].Subject, items.Key,items.IV,items.Salt));

            CurrentUser.Images[CurrentUser.Images.Count - 1].Id = CurrentUser.Images.Count - 1;

            return View("UserGallery", (CurrentUser.Images, Subjects, Enum.GetNames<FilterOption>().ToList()));
        }
        [HttpPost("FilterBySubject")]
        public IActionResult FilterBySubject()
        {
            int? subjectId = Convert.ToInt32(Request.Form["SelectedFilter"]);
            List<TheImage> filteredImages = CurrentUser.Images.Where(image => image.Subject.Name.Equals(Subjects[subjectId.Value].Name)).ToList();
            return View("UserGallery", (filteredImages, Subjects, Enum.GetNames<FilterOption>().ToList()));
        }
        [HttpPost("UnlockImage")]
        public IActionResult UnlockImage()
        {
            int? id = Convert.ToInt32(Request.Form["imageIdFromUnlocking"]);
            string? password = Request.Form["passwordFromUnlocking"];
            if (!_encryptionAndDecryptionService.VerifyHashedPassword(password, CurrentUser.Images[id.Value].Password))
                return View("UserGallery", (CurrentUser.Images, Subjects, Enum.GetNames<FilterOption>().ToList()));

            byte[] decryptedImage = _encryptionAndDecryptionService.DecryptImage(CurrentUser.Images[id.Value].Image, CurrentUser.Images[id.Value].ImageKey, CurrentUser.Images[id.Value].ImageIV);
            CurrentUser.Images[id.Value].Image = decryptedImage;
            CurrentUser.Images[id.Value].IsUnlocked = true;
            return View("UserGallery", (CurrentUser.Images, Subjects, Enum.GetNames<FilterOption>().ToList()));

        }
    }
}
