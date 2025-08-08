using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Datalayer.Models;

namespace Datalayer.Interfaces
{
    public interface ITheImageService
    {
        public bool AddImage(TheImage? image);
        public bool DeleteImage(TheImage? image);
        public bool UpdateImage(TheImage? image);
    }
}
