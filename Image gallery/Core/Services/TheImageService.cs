using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Datalayer.DTO;
using Datalayer.Interfaces;
using Datalayer.Models;

namespace Core.Services
{
    public class TheImageService : ITheImageService
    {
        private readonly HttpClient _httpClient;
        public List<TheImage> Images { get; set; }
        public TheImageService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<bool> AddImage(TheImage image)
        {
            SpecialImageDTO dto = new(image.Id, image.Name, image.Password, image.Image, image.User.Id, image.CreationDate, image.Subject.Id, image.ImageKey, image.ImageIV, image.PasswordSalt);
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7038/api/TheImage/Server/AddImage", dto);
                response.EnsureSuccessStatusCode();
                var item = await response.Content.ReadFromJsonAsync<TheImageDTO>();
                Images.Add(image);
                return true;
            }
            catch (Exception) { return false; }
        }

        public async Task<bool> DeleteImage(TheImage image)
        {
            SpecialImageDTO dto = new(image.Id, image.Name, image.Password, image.Image, image.User.Id, image.CreationDate, image.Subject.Id, image.ImageKey, image.ImageIV, image.PasswordSalt);
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7038/api/TheImage/Server/DeleteImage", dto);
                response.EnsureSuccessStatusCode();
                var item = await response.Content.ReadFromJsonAsync<SpecialImageDTO>();
                Images.Remove(image);
                return true;
            }
            catch (Exception) { return false; }
        }

        public async Task<bool> UpdateImage(TheImage? image)
        {
            SpecialImageDTO dto = new(image.Id, image.Name, image.Password, image.Image, image.User.Id, image.CreationDate, image.Subject.Id, image.ImageKey, image.ImageIV, image.PasswordSalt);
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7038/api/TheImage/Server/UpdateImage", dto);
                var item = await response.Content.ReadFromJsonAsync<SpecialImageDTO>();
                TheImage found = Images.Find(theImage => theImage.Id == image.Id);
                if (found is null)
                    return false;
                found.Name = image.Name;
                found.Password = image.Password;
                found.User.Name = image.User.Name;
                found.User.Password = image.User.Password;
                found.User.Images=image.User.Images;
                found.Subject = image.Subject;
                found.ImageIV = image.ImageIV;
                found.ImageKey = image.ImageKey;
                found.PasswordSalt = image.PasswordSalt;
                found.CreationDate = DateTime.Now;
                found.Image = image.Image;
                return true;
            }
            catch (Exception) { return false; }
        }
        public async Task<List<TheImage>> GetImages()
        {
            try
            {
                List<SpecialImageDTO> special= (await _httpClient.GetFromJsonAsync<List<SpecialImageDTO>>("https://localhost:7038/api/TheImage/Server/GetImages"));
                return special.Select(image => new TheImage(image.Id, image.Name, image.Password, image.Image, new User(image.UserId), image.CreationDate, new Subject(image.SubjectId), image.ImageKey, image.ImageIV, image.PasswordSalt)).ToList();
            }
            catch (Exception ex) { };
            return null;
        }
    }
}
