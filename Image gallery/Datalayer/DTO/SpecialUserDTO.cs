using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalayer.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Datalayer.DTO
{
    public class SpecialUserDTO
    {
        public int Id { get; set; } = default;
        public string? Name { get; set; } = default;
        public string? Password { get; set; } = default;
        public List<int> ImageIds { get; set; } = default;

        public SpecialUserDTO() { }
        public SpecialUserDTO(int id, string? name, string? password, List<int> images)
        {
            Id = id;
            Name = name;
            Password = password;
            ImageIds = images;
        }        
        public SpecialUserDTO(string? name, string? password, List<int> images)
        {
            Name = name;
            Password = password;
            ImageIds = images;
        }
    }
}
