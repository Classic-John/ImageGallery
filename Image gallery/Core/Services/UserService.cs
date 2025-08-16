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

namespace Core.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<UserDTO>> GetUsers()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<UserDTO>>("https://localhost:7038/api/Home/Server/GetUsers");
            }
            catch (Exception ex) {};
            return null;
        }
        public async Task<bool> AddUser(User? user)
        {
            try { await _httpClient.PostAsJsonAsync("https://localhost:7038/api/Home/Server/AddUser", user.toDto()); }
            catch { return false; }
            return true;
        }

        public async Task<bool> DeleteUser(User? user)
        {
            try { await _httpClient.PostAsJsonAsync("https://localhost:7038/api/Home/Server/DeleteUser", user.toDto()); }
            catch { return false; }
            return true;
        }

        public async Task<bool> UpdateUser(User? user)
        {
            try { await _httpClient.PostAsJsonAsync("https://localhost:7038/api/Home/Server/UpdateUser", user.toDto()); }
            catch { return false; }
            return true;
        }
    }
}
