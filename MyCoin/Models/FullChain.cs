using System;
using System.Collections.Generic;
using System.Text;

namespace MyCoin.Models
{
    public class FullChain
    {
        public List<Block> Chain { get; set; }
        public long Length { get { return Chain.Count; } }
    }
}
