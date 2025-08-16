using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalayer.DTO;
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
        public byte[] ImageKey { get; set; }
        public byte[] ImageIV { get; set; }
        public byte[] PasswordSalt { get; set; }
        public TheImage(string? name, string? password, byte[]? image, User? user, DateTime creationDate, Subject? subject, byte[] imageKey, byte[] imageIV, byte[] salt)
        {
            Name = name;
            Password = password;
            Image = image;
            User = user;
            CreationDate = creationDate;
            Subject = subject;
            ImageKey = imageKey;
            ImageIV = imageIV;
            PasswordSalt = salt;
        }
        public TheImage(int id,string? name, string? password, byte[]? image, User? user, DateTime creationDate, Subject? subject, byte[] imageKey, byte[] imageIV, byte[] salt)
        {
            Id=id;
            Name = name;
            Password = password;
            Image = image;
            User = user;
            CreationDate = creationDate;
            Subject = subject;
            ImageKey = imageKey;
            ImageIV = imageIV;
            PasswordSalt = salt;
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
    public static class TheImageExtensions
    {
        public static TheImageDTO toBase(this TheImage image)
            => new TheImageDTO(image.Id, image.Name, image.Password, image.Image, image.User, image.CreationDate, image.Subject, image.ImageKey, image.ImageIV, image.PasswordSalt);
    }
}

