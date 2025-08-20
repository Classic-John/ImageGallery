using System.Collections.Generic;
using Core.Services;
using Datalayer.DTO;
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
        private readonly ISubjectService _subjectService;
        public HomeController(IImageFilterService imageFilterService, ITheImageService theImageService, IUserService userService, IEncryptionAndDecryptionService encryptionAndDecryptionService, ISubjectService subjectService)
        {
            _imageFilterService = imageFilterService;
            _theImageService = theImageService;
            _userService = userService;
            _encryptionAndDecryptionService = encryptionAndDecryptionService;
            _subjectService = subjectService;
        }
        [HttpPost("LoginCheck")]
        public async Task<IActionResult> LoginCheck([FromForm] UserDTO userDTO)
        {
            User user = null;
            List<Subject> subjects = await _subjectService.GetSubjects();
            List<User> users = await _userService.GetUsers();
            List<TheImage> images = await _theImageService.GetImages();

            foreach (Subject sub in subjects)
            {
                foreach (TheImage image in images)
                {
                    if (image.Subject.Id == sub.Id)
                        image.Subject = new(image.Subject.Id, sub.Name);
                }
            }
            foreach (User thisUser in users)
            {
                thisUser.Images = images.FindAll(image => image.User.Id == thisUser.Id);
                images.ForEach(image => image.User = thisUser);
            }
            _userService.Users = users;
            _subjectService.Subjects = subjects;
            _theImageService.Images = images;
            try
            {
                user = _userService.Users.Find(user => user.Name.Equals(userDTO.Name));
                if (user is null)
                    return View("Login");
                if (!_encryptionAndDecryptionService.VerifyHashedPassword(userDTO.Password, user.Password))
                    return View("Login");
            }
            catch (Exception) { return View("Login"); }
            CurrentUser = user;
            IsLoggedIn = true;

            return View("UserGallery", (CurrentUser.Images, _subjectService.Subjects, Enum.GetNames<FilterOption>().ToList()));
        }
        [Route("/")]
        public async Task<IActionResult> Login()
        {
            return View("Login");
        }
        [HttpPost("UserGallery")]
        public async Task<IActionResult> UserGallery()
        {
            return View("UserGallery", (CurrentUser.Images, _subjectService.Subjects, Enum.GetNames<FilterOption>().ToList()));
        }
        public IActionResult LogOff()
        {
            CurrentUser = null;
            return View("Login");
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
        public async Task<IActionResult> AddImage()
        {
            string? name = Request.Form["imageName"];
            DateTime creationTime = DateTime.Now;
            byte[]? image = ConvertImageToBytes(Request.Form.Files["image"]).Result;
            string? password = Request.Form["ImagePassword"];
            string? subject = Request.Form["chosenSubject"];
            SecurityItems items = await _encryptionAndDecryptionService.DeriveKeyAndIVAsync(password);
            byte[] encryptedImage = await _encryptionAndDecryptionService.EncryptImageAsync(image, items.Key, items.IV);
            string? hashedPassword = _encryptionAndDecryptionService.HashPassword(password);
            List<Subject> found = _subjectService.Subjects.FindAll(item => item.Name.Equals(subject));
            if (found.Count < 1)
                return View("UserGallery", (CurrentUser.Images, _subjectService.Subjects, Enum.GetNames<FilterOption>().ToList()));
            TheImage? newImage = new(name, hashedPassword, encryptedImage, CurrentUser, creationTime, found[0], items.Key, items.IV, items.Salt);
            await _theImageService.AddImage(newImage);
            CurrentUser.Images.Add(newImage);
            await _userService.UpdateUser(CurrentUser);
            _theImageService.Images = await _theImageService.GetImages();


            return View("UserGallery", (CurrentUser.Images, _subjectService.Subjects, Enum.GetNames<FilterOption>().ToList()));
        }
        [HttpPost("UpdateImage")]
        public async Task<IActionResult> UpdateImage()
        {
            int? id = Convert.ToInt32(Request.Form["imageId"]);
            string? name = Request.Form["imageName"];
            DateTime creationTime = DateTime.Now;
            string? password = Request.Form["ImagePassword"];
            SecurityItems items = await _encryptionAndDecryptionService.DeriveKeyAndIVAsync(password);
            byte[]? image = await _encryptionAndDecryptionService.EncryptImageAsync(ConvertImageToBytes(Request.Form.Files["image"]).Result, items.Key, items.IV);
            string hashedPassword = _encryptionAndDecryptionService.HashPassword(password);
            TheImage selectedImage = CurrentUser.Images.FirstOrDefault(image => image.Id == id.Value);
            selectedImage = new TheImage(selectedImage.Id, name, password, image, CurrentUser, creationTime, selectedImage.Subject, items.Key, items.IV, items.Salt);
            await _theImageService.UpdateImage(selectedImage);
            await _userService.UpdateUser(CurrentUser);
            _theImageService.Images = await _theImageService.GetImages();
            return View("UserGallery", (CurrentUser.Images, _subjectService.Subjects, Enum.GetNames<FilterOption>().ToList()));
        }
        [HttpPost("DeleteImage")]
        public async Task<IActionResult> DeleteImage()
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
            if (_encryptionAndDecryptionService.VerifyHashedPassword(password, CurrentUser.Images.FirstOrDefault(image => image.Id == id.Value).Password))
                await _theImageService.DeleteImage(CurrentUser.Images.FirstOrDefault(image => image.Id == id.Value));
            await _userService.UpdateUser(CurrentUser);
            _theImageService.Images = await _theImageService.GetImages();
            CurrentUser.Images.Remove(CurrentUser.Images.FirstOrDefault(image => image.Id == id.Value));
            return View("UserGallery", (CurrentUser.Images, _subjectService.Subjects, Enum.GetNames<FilterOption>().ToList()));
        }
        [HttpPost("DuplicateImage")]
        public async Task<IActionResult> DuplicateImage()
        {
            int? id = Convert.ToInt32(Request.Form["imageIdFromDuplication"]);
            string? password = Request.Form["passwordFromDuplication"];
            string? hashedPassword = _encryptionAndDecryptionService.HashPassword(password);
            SecurityItems items = await _encryptionAndDecryptionService.DeriveKeyAndIVAsync(password);
            byte[] image = await _encryptionAndDecryptionService.EncryptImageAsync(CurrentUser.Images.FirstOrDefault(image => image.Id == id.Value).Image,
                items.Key, items.IV);
            TheImage duplicate = new(CurrentUser.Images.FirstOrDefault(image => image.Id == id.Value).Name, hashedPassword,
                image, CurrentUser, DateTime.Now, CurrentUser.Images.FirstOrDefault(image => image.Id == id.Value).Subject,
                items.Key, items.IV, items.Salt);
            CurrentUser.Images.Add(duplicate);
            await _theImageService.AddImage(duplicate);
            await _userService.UpdateUser(CurrentUser);
            _theImageService.Images = await _theImageService.GetImages();

            return View("UserGallery", (CurrentUser.Images, _subjectService.Subjects, Enum.GetNames<FilterOption>().ToList()));
        }
        [HttpPost("AddSubject")]
        public async Task<IActionResult> AddSubject()
        {
            string? subject = Request.Form["subjectName"];
            Subjects.Add(new Subject(subject));
            await _subjectService.AddSubject(new Subject(subject));
            _subjectService.Subjects.Add(new Subject(subject));
            return View("UserGallery", (CurrentUser.Images, _subjectService.Subjects, Enum.GetNames<FilterOption>().ToList()));
        }
        [HttpPost("ApplyImageFilter")]
        public async Task<IActionResult> ApplyImageFilter()
        {
            int? id = Convert.ToInt32(Request.Form["imageIdFromFilter"]);
            int? chosenFilter = Convert.ToInt32(Request.Form["chosenFilter"]);
            string? password = Request.Form["passwordFromFilterImage"];
            byte[] newImage = _imageFilterService.ApplyChosenFilter(CurrentUser.Images.FirstOrDefault(image => image.Id == id.Value).Image, chosenFilter.Value);
            SecurityItems items = await _encryptionAndDecryptionService.DeriveKeyAndIVAsync(password);
            byte[] encryptedImage = await _encryptionAndDecryptionService.EncryptImageAsync(newImage, items.Key, items.IV);
            TheImage filteredImage = new(CurrentUser.Images.FirstOrDefault(image => image.Id == id.Value).Name,
                CurrentUser.Images.FirstOrDefault(image => image.Id == id.Value).Password, encryptedImage, CurrentUser,
                DateTime.Now, CurrentUser.Images.FirstOrDefault(image => image.Id == id.Value).Subject,
                items.Key, items.IV, items.Salt);
            CurrentUser.Images.Add(filteredImage);
            await _theImageService.AddImage(filteredImage);
            await _userService.UpdateUser(CurrentUser);
            _theImageService.Images = await _theImageService.GetImages();
            return View("UserGallery", (CurrentUser.Images, _subjectService.Subjects, Enum.GetNames<FilterOption>().ToList()));
        }
        [HttpPost("ChangeSubject")]
        public async Task<IActionResult> ChangeSubject()
        {
            try
            {
                int? id = Convert.ToInt32(Request.Form["imageIdFromChangeSubject"]);
                string? subject = Request.Form["chosenSubject"];
                Subject sub = (await _subjectService.GetSubjects()).FirstOrDefault(sub => sub.Name.Equals(subject));
                if (sub is null || id is null)
                    return View("UserGallery", (CurrentUser.Images, _subjectService.Subjects, Enum.GetNames<FilterOption>().ToList()));
                TheImage currentImage = (await _theImageService.GetImages()).FirstOrDefault(image => image.Id == id.Value);
                currentImage.Subject = sub;
                await _theImageService.UpdateImage(currentImage);
            }
            catch { return View("UserGallery", (CurrentUser.Images, _subjectService.Subjects, Enum.GetNames<FilterOption>().ToList())); }
            return View("UserGallery", (CurrentUser.Images, _subjectService.Subjects, Enum.GetNames<FilterOption>().ToList()));
        }
        [HttpPost("FilterBySubject")]
        public IActionResult FilterBySubject()
        {
            int? subjectId = Convert.ToInt32(Request.Form["SelectedFilter"]);
            List<TheImage> filteredImages = CurrentUser.Images.Where(image =>
            image.Subject.Name.Equals(_subjectService.Subjects.FirstOrDefault(sub => sub.Id == subjectId.Value).Name)).ToList();
            return View("UserGallery", (filteredImages, _subjectService.Subjects, Enum.GetNames<FilterOption>().ToList()));
        }
        [HttpPost("UnlockImage")]
        public async Task<IActionResult> UnlockImage()
        {
            int? id = Convert.ToInt32(Request.Form["imageIdFromUnlocking"]);
            string? password = Request.Form["passwordFromUnlocking"];
            if (!_encryptionAndDecryptionService.VerifyHashedPassword(password, CurrentUser.Images.FirstOrDefault(image => image.Id == id.Value).Password))
                return View("UserGallery", (CurrentUser.Images, _subjectService.Subjects, Enum.GetNames<FilterOption>().ToList()));

            byte[] decryptedImage = await _encryptionAndDecryptionService.DecryptImageAsync(CurrentUser.Images.FirstOrDefault(image => image.Id == id.Value).Image,
                CurrentUser.Images.FirstOrDefault(image => image.Id == id.Value).ImageKey,
                CurrentUser.Images.FirstOrDefault(image => image.Id == id.Value).ImageIV);
            CurrentUser.Images.FirstOrDefault(image => image.Id == id.Value).Image = decryptedImage;
            CurrentUser.Images.FirstOrDefault(image => image.Id == id.Value).IsUnlocked = true;
            return View("UserGallery", (CurrentUser.Images, _subjectService.Subjects, Enum.GetNames<FilterOption>().ToList()));

        }
        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount([FromForm] SpecialUserDTO userDTO)
        {
            List<User> users = await _userService.GetUsers();
            _userService.Users = users;
            _theImageService.Images = new();
            _subjectService.Subjects = new();
            try
            {
                if (_userService.Users.Find(user => user.Name.Equals(userDTO.Name)) is User)
                    return View("Login");
                userDTO.Password = _encryptionAndDecryptionService.HashPassword(userDTO.Password);
                User user = new User(userDTO.Name, userDTO.Password, new List<TheImage>());
                if ((await _userService.AddUser(user)) is false)
                    return View("Login");
                _userService.Users.Add(user);
            }
            catch (Exception) { return View("Login"); }

            User currentUser = users.FirstOrDefault(user => user.Name.Equals(userDTO.Name));

            CurrentUser = currentUser;
            IsLoggedIn = true;
            _userService.Users = await _userService.GetUsers();
            return View("UserGallery", (CurrentUser.Images, _subjectService.Subjects, Enum.GetNames<FilterOption>().ToList()));
        }
    }
}
