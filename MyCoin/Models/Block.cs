using System;
using System.Collections.Generic;
using System.Text;

namespace MyCoin.Models
{
    public class Block
    {
        public int Index { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<Transaction> Transactions { get; set; }
        public string Proof { get; set; }
        public string PreviousHash { get; set; }
    }
}
