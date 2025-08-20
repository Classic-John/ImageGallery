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
        public List<TheImage> Images { get; set; }
        public Task<List<TheImage>> GetImages();
        public Task<bool> AddImage(TheImage image);
        public Task<bool> DeleteImage(TheImage? image);
        public Task<bool> UpdateImage(TheImage? image);
    }
}
