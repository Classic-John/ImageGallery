using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalayer.Models;

namespace Datalayer.DTO
{
    public class SpecialImageDTO
    {
        public int Id { get; set; } = default;
        public string? Name { get; set; } = default;
        public string? Password { get; set; } = default;
        public byte[]? Image { get; set; } = default;
        public int? UserId { get; set; } = default;
        public DateTime CreationDate { get; set; } = default;
        public int? SubjectId { get; set; } = default;
        [NotMapped]
        public bool IsUnlocked { get; set; } = false;
        public byte[]? ImageKey { get; set; } = default;
        public byte[]? ImageIV { get; set; } = default;
        public byte[]? PasswordSalt { get; set; } = default;
        public SpecialImageDTO() { }
        public SpecialImageDTO(int id, string? name, string? password, byte[]? image, int? user, DateTime creationDate, int? subject, byte[] imageKey, byte[] imageIV, byte[] salt)
        {
            Id = id;
            Name = name;
            Password = password;
            Image = image;
            UserId = user;
            CreationDate = creationDate;
            SubjectId = subject;
            ImageKey = imageKey;
            ImageIV = imageIV;
            PasswordSalt = salt;
        }
    }
}
