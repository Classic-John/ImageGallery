using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datalayer.Models
{
    public class Subject
    {
        string? Name { get; set; }
        public Subject(string? name)
            => Name = name;
    }
}
