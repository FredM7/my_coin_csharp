using System;
using System.Collections.Generic;
using System.Text;

namespace MyCoin.Models
{
    public class ResponseTwo
    {
        public string Message { get; set; }
        public long Index { get; set; }
        public List<Transaction> Transactions { get; set; }
        public string Proof { get; set; }
        public string PreviousHash { get; set; }
    }
}
