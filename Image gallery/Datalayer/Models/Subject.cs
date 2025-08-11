using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalayer.Models.BaseClass;

namespace Datalayer.Models
{
    public class Subject : BaseEntity
    {
        public string? Name { get; set; }
        public Subject(string? name)
            => Name = name;
    }
}
