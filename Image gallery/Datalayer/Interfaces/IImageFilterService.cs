using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datalayer.Interfaces
{
    public interface IImageFilterService
    {
        public byte[] ApplyChosenFilter(byte[]? image, int? filter);
        public Task<byte[]> WaitForFilter(byte[]? image, int? filter);
    }
}
