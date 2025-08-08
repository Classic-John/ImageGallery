using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalayer.Models;

namespace Datalayer.Interfaces
{
    public interface IUserService
    {
        public bool AddUser(User? user);
        public bool DeleteUser(User? user);
        public bool UpdateUser(User? user);
    }
}
