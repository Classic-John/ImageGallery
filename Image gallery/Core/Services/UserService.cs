using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Datalayer.DTO;
using Datalayer.Interfaces;
using Datalayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Core.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        public List<User> Users { get; set; }
        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        private List<User> AddImageIds(List<SpecialUserDTO> special)
        {
            List<TheImage> images = new();
            foreach (SpecialUserDTO item in special)
            {
                foreach (int id in item.ImageIds)
                {
                    images.Add(new TheImage(id));
                }
            }
            return special.Select(user => new User(user.Id, user.Name, user.Password, images)).ToList();
        }
        private User AddImageIdsSingle(SpecialUserDTO special)
        {
            List<TheImage> images = new();
            foreach (int id in special.ImageIds)
            {
                images.Add(new TheImage(id));
            }
            return new User(special.Id, special.Name, special.Password, images);
        }
        public async Task<List<User>> GetUsers()
        {
            try
            {
                List<SpecialUserDTO> special = (await _httpClient.GetFromJsonAsync<List<SpecialUserDTO>>("https://localhost:7038/api/User/Server/GetUsers"));
                return AddImageIds(special);
            }
            catch (Exception ex) { };
            return null;
        }
        public async Task<bool> AddUser(User? user)
        {
            try
            {
                if (user.Images is null)
                    user.Images = new List<TheImage>();
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7038/api/User/Server/AddUser",new SpecialUserDTO(user.Name,user.Password,new List<int>()));
                response.EnsureSuccessStatusCode();
                SpecialUserDTO item = await response.Content.ReadFromJsonAsync<SpecialUserDTO>();
                user.Id = item.Id;
                Users.Add(user);
                return true;
            }
            catch (Exception) { return false; }
        }

        public async Task<bool> DeleteUser(User? user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7038/api/User/Server/DeleteUser", new SpecialUserDTO(user.Name, user.Password, new List<int>()));
                response.EnsureSuccessStatusCode();
                SpecialUserDTO item = await response.Content.ReadFromJsonAsync<SpecialUserDTO>();
                Users.Remove(user);
                return true;
            }
            catch (Exception) { return false; }
        }

        public async Task<bool> UpdateUser(User? user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7038/api/User/Server/UpdateUser", new SpecialUserDTO(user.Id,user.Name, user.Password, new List<int>()));
                var item = await response.Content.ReadFromJsonAsync<SpecialUserDTO>();
                User found = Users.Find(theUser => theUser.Id == user.Id);
                if (found is null)
                    return false;
                found.Name = item.Name;
                found.Password = item.Password;
                found.Images=user.Images;
                return true;
            }
            catch (Exception) { return false; }
        }
        public async Task<User> FindUser(int? id)
        {
            if (id == null)
                return null;

            try
            {
                SpecialUserDTO special = await _httpClient.GetFromJsonAsync<SpecialUserDTO>($"https://localhost:7038/api/User/Server/FindUser/{id}");
                return AddImageIdsSingle(special);
            }
            catch (Exception) { };
            return null;

        }
    }
}
