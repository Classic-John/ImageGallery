using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datalayer.Models
{
    public class SecurityItems
    {
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
        public byte[]? Salt { get; set; }
        
        public SecurityItems(byte[] key, byte[] iv, byte[] salt) 
        {
            Key = key;
            IV = iv;
            Salt = salt;
        }
    }
}
