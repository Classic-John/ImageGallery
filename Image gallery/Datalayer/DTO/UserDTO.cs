using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalayer.Models;

namespace Datalayer.DTO
{
    public class UserDTO
    {
        public int Id { get; set; } = default;
        public string? Name { get; set; }
        public string? Password { get; set; }
        public List<TheImage> Images { get; set; }

        public UserDTO(int id ,string? name, string? password, List<TheImage> images)
        {
            Id = id;
            Name = name;
            Password = password;
            Images = images;
        }
    }
    public static class UserDTOExtensions
    {
        public static User toBase(this UserDTO user)
        {
            return new User(user.Id, user.Name, user.Password, user.Images);
        }
    }
}
