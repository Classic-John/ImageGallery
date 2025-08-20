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
    public class SubjectService : ISubjectService
    {
        private readonly HttpClient _httpClient;
        public List<Subject> Subjects { get; set; }
        public SubjectService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> AddSubject(Subject? subject)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7038/api/Subject/Server/AddSubject", subject.toDTO());
                response.EnsureSuccessStatusCode();
                var item = await response.Content.ReadFromJsonAsync<SubjectDTO>();
                Subjects.Add(subject);
                return true;
            }
            catch (Exception) { return false; }
        }

        public async Task<bool> DeleteSubject(Subject? subject)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7038/api/Subject/Server/DeleteSubject", subject.toDTO());
                response.EnsureSuccessStatusCode();
                var item = await response.Content.ReadFromJsonAsync<SubjectDTO>();
                Subjects.Remove(subject);
                return true;
            }
            catch (Exception) { return false; }
        }

        public async Task<List<Subject>> GetSubjects()
        {
            try
            {
                return (await _httpClient.GetFromJsonAsync<List<SubjectDTO>>("https://localhost:7038/api/Subject/Server/GetSubjects")).
                    Select(subject => subject.toBase()).ToList();
            }
            catch (Exception ex) { };
            return null;
        }

        public async Task<bool> UpdateSubject(Subject? subject)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7038/api/Subject/Server/UpdateImage", subject.toDTO());
                var item = await response.Content.ReadFromJsonAsync<TheImageDTO>();
                Subject found = Subjects.Find(theImage => theImage.Id == subject.Id);
                if (found is null)
                    return false;
                found.Name = item.Name;
                return true;
            }
            catch (Exception) { return false; }
        }
    }
}
