using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalayer.Models.BaseClass;

namespace Datalayer.Models
{
    public class TheImage : BaseEntity
    {
        public string? Name { get; set; }
        public string? Password { get; set; }
        public byte[]? Image { get; set; }
        public User? User { get; set; }
        public DateTime CreationDate { get; set; }
        public Subject? Subject { get; set; }
        public TheImage(string? name, string? password, byte[]? image, User? user, DateTime creationDate, Subject? subject)
        {
            Name = name;
            Password = password;
            Image = image;
            User = user;
            CreationDate = creationDate;
            Subject = subject;
        }
        public TheImage(string? name, string? password, byte[]? image, User? user, DateTime creationDate)
        {
            Name = name;
            Password = password;
            Image = image;
            User = user;
            CreationDate = creationDate;
        }
    }
}
