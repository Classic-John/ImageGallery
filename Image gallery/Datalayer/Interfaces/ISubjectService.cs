using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalayer.Models;

namespace Datalayer.Interfaces
{
    public interface ISubjectService
    {
        public List<Subject> Subjects { get; set; }
        public Task<List<Subject>> GetSubjects();
        public Task<bool> AddSubject(Subject? subject);
        public Task<bool> UpdateSubject(Subject? subject);
        public Task<bool> DeleteSubject(Subject? subject);
    }
}
