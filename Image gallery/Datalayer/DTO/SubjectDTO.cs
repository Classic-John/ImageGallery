using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Datalayer.Models;

namespace Datalayer.DTO
{
    public class SubjectDTO
    {
        public int Id { get; set; } = default;
        public string? Name { get; set; }
        public SubjectDTO() { }
        public SubjectDTO(string? name)
            => Name = name;
        public SubjectDTO(int id, string? name)
        {
            Id=id;
            Name = name;
        }
    }
    public static class SubjectDTOExtensions
    {
        public static Subject toBase(this SubjectDTO dto)
           => new Subject(dto.Id, dto.Name);
    }
}
