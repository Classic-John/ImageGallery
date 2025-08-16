using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalayer.DTO;
using Datalayer.Models.BaseClass;

namespace Datalayer.Models
{
    public class Subject : BaseEntity
    {
        public string? Name { get; set; }
        public Subject(string? name)
            => Name = name;
        public Subject(int id, string? name)
        {
            Id = id;
            Name = name;
        }
    }
    public static class SubjectExtensions
    {
        public static SubjectDTO toDTO(this Subject subject)
            => new SubjectDTO(subject.Id,subject.Name);
    }
}
