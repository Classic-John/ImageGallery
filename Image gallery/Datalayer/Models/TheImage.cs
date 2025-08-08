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
        public int CreatorId { get; set; }
        public DateTime CreationDate { get; set; }
        public TheImage(string? name, string? password, byte[]? image, int creatorId, DateTime creationDate)
        {
            Name = name;
            Password = password;
            Image = image;
            CreatorId = creatorId;
            CreationDate = creationDate;
        }
    }
}
