using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalayer.DTO;
using Datalayer.Models;

namespace Datalayer.Interfaces
{
    public interface IUserService
    {
        public Task<List<UserDTO>> GetUsers();
        public Task<bool> AddUser(User? user);
        public Task<bool> DeleteUser(User? user);
        public Task<bool> UpdateUser(User? user);
    }
}
