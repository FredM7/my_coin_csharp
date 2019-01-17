using System;
using System.Collections.Generic;
using System.Text;

namespace MyCoin.Models
{
    public class Transaction
    {
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public decimal Amount { get; set; }
    }
}
