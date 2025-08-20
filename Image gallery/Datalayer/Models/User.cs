using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Datalayer.DTO;
using Datalayer.Models.BaseClass;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Datalayer.Models
{
    public class User : BaseEntity
    {
        public string? Name { get; set; }
        public string? Password { get; set; }
        public List<TheImage> Images { get; set; }

        public User(int? userId)
            => Id = userId ?? default;
        public User(string? name, string? password, List<TheImage> images)
        {
            Name = name;
            Password = password;
            Images = images;
        }
        public User(int id, string? name, string? password, List<TheImage> images)
        {
            Id = id;
            Name = name;
            Password = password;
            Images = images;
        }
    }
    public static class UserExtensions
    {
        public static UserDTO toDto(this User user)
        {
            return new UserDTO(user.Id, user.Name, user.Password, user.Images);
        }
    }
}
