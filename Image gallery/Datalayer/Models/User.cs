using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalayer.Models.BaseClass;

namespace Datalayer.Models
{
    public class User : BaseEntity
    {
        public string? Name { get; set; }
        public string? Password { get; set; }
        public List<TheImage> Images { get; set; }

        public User(string? name, string? password, List<TheImage> images)
        {
            Name = name;
            Password = password;
            Images = images;
        }
    }
}
