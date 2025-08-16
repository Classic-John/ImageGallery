using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Datalayer.Models;

namespace Datalayer.DTO
{
    public class TheImageDTO
    {
        public int Id { get; set; } = default;
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
        public TheImageDTO(string? name, string? password, byte[]? image, User? user, DateTime creationDate, Subject? subject, byte[] imageKey, byte[] imageIV, byte[] salt)
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
        public TheImageDTO(int id, string? name, string? password, byte[]? image, User? user, DateTime creationDate, Subject? subject, byte[] imageKey, byte[] imageIV, byte[] salt)
        {
            Id = id;
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
        public TheImageDTO(string? name, string? password, byte[]? image, User? user, DateTime creationDate)
        {
            Name = name;
            Password = password;
            Image = image;
            User = user;
            CreationDate = creationDate;
        }
    }

    public static class TheImageDTOExtensions
    {
        public static TheImage toBase(this TheImageDTO image)
            => new TheImage(image.Id, image.Name, image.Password, image.Image, image.User, image.CreationDate, image.Subject, image.ImageKey, image.ImageIV, image.PasswordSalt);
    }
}
