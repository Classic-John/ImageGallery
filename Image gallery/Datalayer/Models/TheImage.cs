using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        [NotMapped]
        public bool IsUnlocked { get; set; } = false;
        public byte[] imageKey { get; set; }
        public byte[] imageIV { get; set; }
        public TheImage(string? name, string? password, byte[]? image, User? user, DateTime creationDate, Subject? subject, byte[] imageKey, byte[] imageIV)
        {
            Name = name;
            Password = password;
            Image = image;
            User = user;
            CreationDate = creationDate;
            Subject = subject;
            this.imageKey = imageKey;
            this.imageIV = imageIV;
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
